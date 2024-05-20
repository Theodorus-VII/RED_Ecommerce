using Ecommerce.Configuration;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Checkout;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using Ecommerce.Services.ShoppingCart;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Reflection;

using Ecommerce.Services.Payment;
using DotNetEnv;
using Ecommerce.Services.Orders;
using Ecommerce.Middleware;
using Sentry.Profiling;
using Sentry;


Env.Load();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

builder.Services.AddControllers();

// Configure the app to use sentry.io
// Load Configuration from environment
var sentryConfig = builder.Configuration
    .GetSection("SentryConfiguration")
    .Get<SentryConfiguration>() 
    ?? throw new Exception(
        "Sentry configuration not found. Make sure the environment variables are configured properly");

builder.Services.AddSingleton(sentryConfig);

builder.WebHost.UseSentry(
    o =>
        {
            o.Dsn = sentryConfig.Dsn;
            o.Debug = sentryConfig.Debug;
            o.EnableTracing = sentryConfig.EnableTracing;
            o.IsGlobalModeEnabled = sentryConfig.IsGlobalModeEnabled;
            o.TracesSampleRate = sentryConfig.TracesSampleRate;
            o.ProfilesSampleRate = sentryConfig.ProfilesSampleRate;
            o.AddIntegration(new ProfilingIntegration(
                TimeSpan.FromMilliseconds(500)
            ));
        }
);

SentrySdk.CaptureMessage("Hello Sentry");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ecommerce API",
        Version = "v1"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Automapper.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add the HttpClient for sending http requests from the server.
builder.Services.AddHttpClient();

// Old MySql Connection config.
// var connectionString = @"Server=(localdb)\mssqllocaldb;Database=EcommerceTest";
// builder.Services.AddDbContext<ApplicationDbContext>(
//     options => options.UseSqlServer(connectionString)
// );
// var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");


var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(
        connectionString,
        npgsqlOptionsAction:
            npgSqlOptions => npgSqlOptions.EnableRetryOnFailure(maxRetryCount: 50))
);

builder.Services.AddJwtAuthentication(builder.Configuration);

// Load Configuration for the email service.
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Add Services.
builder.Services.AddScoped<TestService>();  // Sample Service.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IProductService, ProductService>();


// Add middleware.
builder.Services.AddTransient<ExtractUserIdMiddleware>();
builder.Services.AddTransient<ErrorHandlingMiddleware>();


var app = builder.Build();

app.Logger.LogInformation("Application Created...");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            // swagger ui is available at the root of the application.
            // options.RoutePrefix = string.Empty;
        }
    );
}
// Add Https auto-redirection.
app.UseHttpsRedirection();

// Configure authentication and authorization.
app.UseAuthentication();
app.UseAuthorization();

// Register middleware for use.
app.UseMiddleware<ExtractUserIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure CORS.
app.UseCors(
    options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
);

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        app.Logger.LogWarning("Found pending Db migrations.");
        app.Logger.LogInformation("Attempting to apply pending migrations...");

        context.Database.Migrate();
    }
    else
    {
        app.Logger.LogInformation("Found no pending migrations. Proceeding...");
    }
}

// serve static images.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "./Public/Images")),
    RequestPath = "/images"
});

// .well-known (for flutter deep link).
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "./.well-known")),
    RequestPath = "/.well-known"
});

// Predefining roles in the database    
app.Logger.LogInformation("Creating roles...");
using (var scope = app.Services.CreateScope())
{
    var roles = new string[] { Roles.Admin, Roles.Customer };
    await scope.ServiceProvider.AddRoles(roles);
}
app.Logger.LogInformation("Roles created.");

// Seeding the database with sample data (Using ApplicationDbContextSeed.cs).
app.Logger.LogInformation("Seeding the database...");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.InitializeDb();
}
app.Logger.LogInformation("Database seeded");


app.Logger.LogInformation("Starting Server...");
app.Run();