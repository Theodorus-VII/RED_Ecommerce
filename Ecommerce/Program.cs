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


var builder = WebApplication.CreateBuilder(args);

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



// var connectionString = @"Server=(localdb)\mssqllocaldb;Database=EcommerceTest";
// builder.Services.AddDbContext<ApplicationDbContext>(
//     options => options.UseSqlServer(connectionString)
// );
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(
        connectionString,
        npgsqlOptionsAction:
            npgSqlOptions => npgSqlOptions.EnableRetryOnFailure(maxRetryCount: 10)
    )
);

builder.Services.AddJwtAuthentication(builder.Configuration);

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Add the services here. Same format,
//  just replace TestService with the service to use.
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddTransient<ExtractUserIdMiddleware>();


var app = builder.Build();

app.Logger.LogInformation("Application Created...");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExtractUserIdMiddleware>();

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

// Predefining roles in the database    
app.Logger.LogInformation("Creating roles...");
using (var scope = app.Services.CreateScope())
{
    var roles = new string[] { Roles.Admin, Roles.Customer };
    await scope.ServiceProvider.AddRoles(roles);
}

app.Logger.LogInformation("Roles created.");

app.Logger.LogInformation("Starting app...");

app.Run();