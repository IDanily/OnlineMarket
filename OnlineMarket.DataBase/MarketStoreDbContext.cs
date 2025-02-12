using Microsoft.EntityFrameworkCore;
using OnlineMarket.DataBase.Configurations;
using OnlineMarket.DataBase.Entites;

namespace OnlineMarket.DataBase
{
    public class MarketStoreDbContext : DbContext
    {
        public MarketStoreDbContext(DbContextOptions<MarketStoreDbContext> options) : base(options)
        {

        }

        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<UsersEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<NotificationEntity> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfigurator());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new UsersConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfigurator());

            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => op.Id);

            modelBuilder.Entity<OrderProduct>().Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProduct)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProduct)
                .HasForeignKey(op => op.ProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
