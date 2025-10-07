using System.Text;
using BankingAppProjectWebAPI.Data;
using BankingAppProjectWebAPI.Models;
using BankingAppProjectWebAPI.Services.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BankingAppProjectWebAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection InitializeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(Program));
        services.AddScoped<AccountService>();
        services.AddScoped<BankAccountService>();
        services.AddScoped<PasswordHasher<AccountInformation>>();

        services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "main");
                }));
        var jwtSettings = configuration.GetSection("Jwt");
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
                };
            });

        services.AddControllers();
        services.AddOpenApi();
        services.AddAuthorization();

        return services;
    }
}