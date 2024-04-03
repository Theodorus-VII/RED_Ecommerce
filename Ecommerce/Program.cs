using Ecommerce.Configuration;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



// var connectionString = @"Server=(localdb)\mssqllocaldb;Database=EcommerceTest";
// builder.Services.AddDbContext<ApplicationDbContext>(
//     options => options.UseSqlServer(connectionString)
// );
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = builder.Configuration.GetConnectionString("DockerConnection");

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mySqlOptionsAction: mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            }
        );
    }
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

app.Logger.LogInformation("Creating roles...");

using (var scope = app.Services.CreateScope())
{
    var roles = new string[] { Roles.Admin, Roles.Customer };
    await scope.ServiceProvider.AddRoles(roles);
}

app.Logger.LogInformation("Roles created.");

app.Logger.LogInformation("Starting app...");

app.Run();