using BibliotecaAppV2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BibliotecaAppV2.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "Admin", "Funcionario", "Membro" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        // NOVO MÉTODO: Cria um admin padrão, com hash seguro na senha
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string adminEmail = "admin@biblioteca.com";
            string adminPassword = "Admin@123"; // Troque depois para algo mais seguro!
            string adminName = "Administrador";

            // Verifica se já existe um admin com esse email
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NomeCompleto = adminName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword); // Aqui já faz o HASH!
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                // Se quiser, trate erros de criação aqui
            }
        }
    }
}