
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using pfm.Database.Entities;

namespace pfm.Database;

public class PFMDbContext : DbContext
{

    public DbSet<TransactionEntity> transactions { get; set; }
    public DbSet<CategoryEntity> categories { get; set; }
    public DbSet<TransactionSplitsEntity> transaction_splits { get; set; }

    public PFMDbContext()
    {

    }

    public PFMDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}