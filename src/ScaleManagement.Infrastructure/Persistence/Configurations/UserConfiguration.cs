using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName).HasMaxLength(128).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.DisplayName).HasMaxLength(256).IsRequired();
        builder.Property(u => u.ExternalAuthId).HasMaxLength(256);
        builder.Property(u => u.Role).HasConversion<int>();
        builder.Property(u => u.CreatedBy).HasMaxLength(256);
        builder.Property(u => u.ModifiedBy).HasMaxLength(256);
        builder.Property(u => u.DeletedBy).HasMaxLength(256);

        builder.HasIndex(u => new { u.TenantId, u.UserName }).IsUnique();
        builder.HasIndex(u => u.ExternalAuthId);
    }
}
