using Microsoft.EntityFrameworkCore;

namespace WebApiAuthorsNet5
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
