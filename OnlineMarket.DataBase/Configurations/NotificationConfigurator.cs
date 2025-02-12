using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineMarket.DataBase.Entites;

namespace OnlineMarket.DataBase.Configurations
{
    public class NotificationConfigurator : IEntityTypeConfiguration<NotificationEntity>
    {
        public void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(e => e.User)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            builder.Property(b => b.Message).IsRequired();
            builder.Property(b => b.CreatedAt);
            builder.Property(b => b.IsRead);
        }
    }
}
