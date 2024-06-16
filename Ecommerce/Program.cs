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
using System.Runtime.CompilerServices;
using System.Reflection;
using Ecommerce.Services.Payment;
using DotNetEnv;
using Ecommerce.Services.Orders;
using Microsoft.OpenApi.Models;
using System.Reflection;


Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

// Add services to the container.

builder.Services.AddControllers();

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
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient();



// var connectionString = @"Server=(localdb)\mssqllocaldb;Database=EcommerceTest";
// builder.Services.AddDbContext<ApplicationDbContext>(
//     options => options.UseSqlServer(connectionString)
// );
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
//        npgsqlOptionsAction:
//            npgSqlOptions => npgSqlOptions.EnableRetryOnFailure(maxRetryCount: 50)
//    )
//);

builder.Services.AddJwtAuthentication(builder.Configuration);

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);


// Add the services here. Same format,
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
//  just replace TestService with the service to use.
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();



builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IProductService,ProductService>();

builder.Services.AddTransient<ExtractUserIdMiddleware>();

app.Logger.LogInformation("Application Created...");


app.UseSwagger();
app.UseSwaggerUI();

var app = builder.Build();

    app.UseSwaggerUI(
        options =>
        {
            // options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            // options.RoutePrefix = string.Empty;
        }
    );
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

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
        app.Logger.LogInformation("Found no pending migrations.");
    }
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "./Public/Images")),
    RequestPath = "/images"
});
app.UseMiddleware<ExtractUserIdMiddleware>();
app.UseStaticFiles(new StaticFileOptions{

// Predefining roles in the database    
app.Logger.LogInformation("Creating roles...");
    FileProvider=new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath,"./Public/Images")),
    RequestPath="/images"

});
using (var scope = app.Services.CreateScope())

app.Logger.LogInformation("Roles created.");

app.Logger.LogInformation("Seeding the database...");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.InitializeDb();
}
{
app.Logger.LogInformation("Database seeded");

app.Logger.LogInformation("Starting app...");
    await scope.ServiceProvider.AddRoles(roles);
}

app.MapControllers();

app.Run();