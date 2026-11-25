using Microsoft.EntityFrameworkCore;

namespace CustomerDesk.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers
    {
        get { return Set<Customer>(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Customer>().HasIndex(c => c.Email)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = 0");
        modelBuilder.Entity<Customer>().HasIndex(c => c.Phone)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = 0");
    }
}
