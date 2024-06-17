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
using Ecommerce.Services.Payment;
using DotNetEnv;
using Ecommerce.Services.Orders;
using Microsoft.OpenApi.Models;
using System.Reflection;


Env.Load();
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

builder.Services.AddJwtAuthentication(builder.Configuration);

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);


// Add the services here. Same format,
//  just replace TestService with the service to use.
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();



builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddTransient<ExtractUserIdMiddleware>();



var app = builder.Build();

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
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "./Public/Images")),
    RequestPath = "/images"

});
using (var scope = app.Services.CreateScope())
{
    var roles = new string[] { Roles.Admin, Roles.Customer };
    await scope.ServiceProvider.AddRoles(roles);
}

app.MapControllers();

app.Run();