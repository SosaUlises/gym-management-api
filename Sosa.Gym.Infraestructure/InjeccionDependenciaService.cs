using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sosa.Gym.Application.DataBase;
using Sosa.Gym.Application.External;
using Sosa.Gym.Domain.Entidades.Usuario;
using Sosa.Gym.Infraestructure.DataBase;
using Sosa.Gym.Infraestructure.Email;
using Sosa.Gym.Infraestructure.GetTokenJWT;
using System.Security.Claims;
using System.Text;

namespace Sosa.Gym.Infraestructure
{
    public static class InjeccionDependenciaService
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services,
        IConfiguration configuration)
        {

            // Conexion a PostgreSQL

            var connectionString = configuration.GetConnectionString("PostgreConnectionString");
            services.AddDbContext<DataBaseService>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IDataBaseService, DataBaseService>();


            // Identity

            services.AddIdentity<UsuarioEntity, IdentityRole<int>>(options =>
            {
                // Password
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Lockout (anti fuerza bruta)
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
                .AddEntityFrameworkStores<DataBaseService>()
                .AddDefaultTokenProviders();



            // JWT – Configuración completa

            var jwtKey = configuration["Jwt_Key"];
            var jwtIssuer = configuration["Jwt_Issuer"];
            var jwtAudience = configuration["Jwt_Audience"];

            if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            {
                Console.WriteLine($"[ERROR CRÍTICO] La Jwt_Key es nula o muy corta. Valor leído: '{jwtKey}'");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

            });


            // Servicios

            services.AddScoped<IGetTokenJWTService, GetTokenJWTService>();
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.AddScoped<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}
