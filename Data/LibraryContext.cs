using BibliotekosValdymoSistema.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotekosValdymoSistema.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<LibraryWorker> LibraryWorkers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LibraryWorker>()
                .HasMany(lw => lw.Books)
                .WithMany(b => b.LibraryWorkers)
                .UsingEntity(j => j.ToTable("LibraryWorkerBooks"));
        }
    }
}
