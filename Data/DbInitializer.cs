using BibliotekaOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaOnline.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            if (await userManager.FindByEmailAsync("admin@admin.com") == null)
            {
                var admin = new User { UserName = "admin@admin.com", Email = "admin@admin.com", EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync("user@user.com") == null)
            {
                var user = new User { UserName = "user@user.com", Email = "user@user.com", EmailConfirmed = true };
                await userManager.CreateAsync(user, "User123!");
                await userManager.AddToRoleAsync(user, "User");
            }

            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    new Book { Title = "Wiedźmin", Author = "Andrzej Sapkowski", Description = "Saga fantasy.", TotalCopies = 3 },
                    new Book { Title = "Lalka", Author = "Bolesław Prus", Description = "Powieść realistyczna.", TotalCopies = 2 },
                    new Book { Title = "Pan Tadeusz", Author = "Adam Mickiewicz", Description = "Epopeja narodowa.", TotalCopies = 1 }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}