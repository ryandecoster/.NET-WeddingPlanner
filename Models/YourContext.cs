using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{
    public class YourContext : DbContext
    {
        public YourContext(DbContextOptions<YourContext> options) : base(options) {}
        public DbSet<User> Users { get; set; } 
        public DbSet<Wedding> Weddings { get; set; } 
        public DbSet<Invite> Invites { get; set; } 
    }
}