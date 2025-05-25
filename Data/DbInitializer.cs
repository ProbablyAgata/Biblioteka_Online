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
                    new Book { Title = "Wiedźmin 3. Krew elfów", Author = "Andrzej Sapkowski", Description = "Saga fantasy.", TotalCopies = 3, Publisher = "SuperNova", Categories = "Fantasy", Language = "Polski", PublishedDate = "2014", ISBN = "978-83-7578-065-9", PageCount = 340 },
                    new Book { Title = "Lalka", Author = "Bolesław Prus", Description = "Powieść realistyczna.", TotalCopies = 2, Publisher = "Siedmioróg", Categories = "Powieść", Language = "Polski", PublishedDate = "2017", ISBN = "978-83-7791-604-9", PageCount = 682},
                    new Book { Title = "Pan Tadeusz", Author = "Adam Mickiewicz", Description = "Epopeja narodowa.", TotalCopies = 1, Publisher = "Greg", Categories = "Poemat epicki", Language = "Polski", PublishedDate = "2003", ISBN = "83-7327-228-3", PageCount = 335 }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}