using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
        builder.Property(c => c.Code).HasMaxLength(64).IsRequired();
        builder.Property(c => c.Type).HasConversion<int>();
        builder.Property(c => c.ContactName).HasMaxLength(256);
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.Phone).HasMaxLength(64);
        builder.Property(c => c.AddressLine1).HasMaxLength(256);
        builder.Property(c => c.AddressLine2).HasMaxLength(256);
        builder.Property(c => c.City).HasMaxLength(128);
        builder.Property(c => c.StateOrProvince).HasMaxLength(128);
        builder.Property(c => c.PostalCode).HasMaxLength(32);
        builder.Property(c => c.Country).HasMaxLength(128);
        builder.Property(c => c.TaxId).HasMaxLength(64);
        builder.Property(c => c.AccountNumber).HasMaxLength(64);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.ModifiedBy).HasMaxLength(256);
        builder.Property(c => c.DeletedBy).HasMaxLength(256);

        builder.HasIndex(c => new { c.TenantId, c.Code }).IsUnique();

        builder.HasMany(c => c.Drivers)
            .WithOne(d => d.Company!)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Trucks)
            .WithOne(t => t.Company!)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
