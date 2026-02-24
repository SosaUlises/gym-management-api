using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sosa.Gym.Domain.Entidades.Usuario;

namespace Sosa.Gym.Infraestructure.Seed
{
    public class IdentityDataSeed
    {
        public static async Task SeedRolesAsync(IHost app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsuarioEntity>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string[] roles = { "Cliente", "Entrenador", "Administrador" };


            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var res = await roleManager.CreateAsync(new IdentityRole<int>(role));
                    if (!res.Succeeded)
                    {
                        throw new Exception("No se pudo crear el rol: " +
                            string.Join(", ", res.Errors.Select(e => e.Description)));
                    }
                }
            }

            var adminEmail = config["Admin_Email"]?.Trim();
            var adminPassword = config["Admin_Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                throw new Exception("Debes configurar Admin_Email y Admin_Password como secreto");

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new UsuarioEntity
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    Nombre = "Admin",
                    Apellido = "Sistema",
                    Dni = 99999999,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createResult.Succeeded)
                {
                    throw new Exception("No se pudo crear el usuario administrador: " +
                                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Administrador");
                if (!addRoleResult.Succeeded)
                {
                    throw new Exception("No se pudo asignar rol Administrador: " +
                                        string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
