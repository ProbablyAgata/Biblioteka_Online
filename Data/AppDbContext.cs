using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BibliotekaOnline.Models;

namespace BibliotekaOnline.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Borrowing> Borrowings => Set<Borrowing>();
    }
}