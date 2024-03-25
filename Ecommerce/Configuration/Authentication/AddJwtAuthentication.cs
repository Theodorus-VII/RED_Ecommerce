using System.Text;
using Ecommerce.Data;
using Ecommerce.Infrastructure.Authentication;
using Ecommerce.Models;
using Ecommerce.Services.Authentication;
using Ecommerce.Services.Inerfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Configuration;


public static class ConfigureJwtAuthentication
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        

        services.AddIdentity<User, IdentityRole<Guid>>(
            options =>
            {
                // options.Password.RequiredLength = 7;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }
        ).AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            }
        );
        
        // services.AddAuthorization(
        //     options =>
        //     {
        //         options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        //     }
        // );

        return services;
    }

}