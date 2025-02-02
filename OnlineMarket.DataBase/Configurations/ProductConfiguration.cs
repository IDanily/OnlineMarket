using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineMarket.DataBase.Entites;

namespace OnlineMarket.DataBase.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId);

            builder.HasOne(p => p.Seller)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.SellerId);

            builder.Property(b => b.Description).IsRequired(false);
            builder.Property(b => b.Name).IsRequired(false);
            builder.Property(b => b.Number).IsRequired(false);
            builder.Property(p => p.Price).HasPrecision(18, 2);
        }
    }
}
