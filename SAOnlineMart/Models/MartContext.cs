using Microsoft.EntityFrameworkCore;

namespace SAOnlineMart.Models
{
    public class MartContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Cart> Cart { get; set; }

        public MartContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
