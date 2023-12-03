using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data.Models;

namespace Data
{
    public class ShoppingAppDbContext : DbContext
    {
        public ShoppingAppDbContext()
        {
        }
        public ShoppingAppDbContext(DbContextOptions<ShoppingAppDbContext> options) : base(options)
        {
        }
        public DbSet<ProductDTO> Products { get; set; }

        public DbSet<OrderHeaderDTO> OrderHeaders { get; set; }

        public DbSet<OrderLineDTO> OrderLines { get; set; }

        public DbSet<AccountDTO> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDTO>().ToTable("Products").HasKey(p => p.Uid);
            modelBuilder.Entity<OrderHeaderDTO>().ToTable("OrderHeaders").HasKey(oh => oh.Uid);
            modelBuilder.Entity<OrderLineDTO>().ToTable("OrderLines").HasKey(ol => ol.Uid);
            modelBuilder.Entity<AccountDTO>().ToTable("Accounts").HasKey(a => a.Uid);
            modelBuilder.Entity<AccountDTO>()
                .HasIndex(a => a.Email)
                .IsUnique();
            modelBuilder.Entity<OrderHeaderDTO>()
                .HasMany(e => e.OrderLines)
                .WithOne(e => e.OrderHeader)
                .HasForeignKey(e => e.OrderHeaderUid)
                .HasPrincipalKey(e => e.Uid);

            modelBuilder.Entity<ProductDTO>()
                .HasMany(e => e.OrderLines)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductUid)
                .HasPrincipalKey(e => e.Uid);

            modelBuilder.Entity<AccountDTO>()
               .HasMany(e => e.OrderHeaders)
               .WithOne(e => e.Account)
               .HasForeignKey(e => e.AccountId)
               .HasPrincipalKey(e => e.Uid);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DBCONNECTIONSTRING"));
            }
        }
    }
}

