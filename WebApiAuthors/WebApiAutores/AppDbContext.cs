using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors
{
    
    // public class AppDbContext : DbContext

    //  cambiamos de clase xq queremos usar Identity
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // creamos una tabla intermedia
            modelBuilder.Entity<AuthorBook>()
                .HasKey(ab => new { ab.BookId, ab.AuthorId });
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AuthorBook> AuthorsBooks { get; set; }
    }
}
