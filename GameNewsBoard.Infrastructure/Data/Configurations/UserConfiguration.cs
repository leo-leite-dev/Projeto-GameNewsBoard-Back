using GameNewsBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameNewsBoard.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .IsRequired();

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.SteamId)
                .HasMaxLength(50);

            builder.HasIndex(u => u.Username)
               .IsUnique();

            builder.HasIndex(u => u.SteamId)
               .IsUnique()
               .HasFilter("\"SteamId\" IS NOT NULL");

            builder.HasMany(u => u.TierLists)
               .WithOne(t => t.User)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.GameStatuses)
                .WithOne(gs => gs.User)
                .HasForeignKey(gs => gs.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}