using Microsoft.EntityFrameworkCore;
using WarehouseApp.Models;

namespace WarehouseApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Resource> Resource => Set<Resource>();
        public DbSet<Unit> Unit => Set<Unit>();
        public DbSet<Receipt> Receipt => Set<Receipt>();
        public DbSet<ReceiptItem> ReceiptItem => Set<ReceiptItem>();
        public DbSet<StockBalance> StockBalance => Set<StockBalance>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Resource>().HasIndex(x => x.Name).IsUnique();
            mb.Entity<Unit>().HasIndex(x => x.Name).IsUnique();
            mb.Entity<Receipt>().HasIndex(x => x.Number).IsUnique();
           
            mb.Entity<ReceiptItem>()
                .HasIndex(x => new {x.ReceiptId, x.ResourceId, x.UnitId}).IsUnique();

            mb.Entity<StockBalance>()
                .HasIndex(x => new { x.ResourceId, x.UnitId }).IsUnique();

            mb.Entity<ReceiptItem>().Property(p => p.Quantity).HasPrecision(18, 6);
            mb.Entity<StockBalance>().Property(p => p.Quantity).HasPrecision(18, 6);
        }
    }
}
