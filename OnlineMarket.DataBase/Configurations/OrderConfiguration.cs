using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineMarket.DataBase.Entites;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMarket.DataBase.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(e => e.Users)
                .WithOne(e => e.Order)
                .HasForeignKey<OrderEntity>(e => e.UsersId)
                .IsRequired();

            builder.Property(b => b.Description).IsRequired(false);
            builder.Property(b => b.Name).IsRequired(false);
            builder.Property(b => b.Number).IsRequired(false);
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(b => b.DateCreate).IsRequired(false);
            builder.Property(b => b.DateExpiration).IsRequired(false);
        }
    }
}
