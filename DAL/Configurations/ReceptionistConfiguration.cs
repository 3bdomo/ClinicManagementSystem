using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class ReceptionistConfiguration : IEntityTypeConfiguration<Receptionist>
{
    public void Configure(EntityTypeBuilder<Receptionist> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.ApplicationUserId)
            .IsRequired();

        builder.Property(r => r.IsActive)
            .HasDefaultValue(true);


        builder.Property(r => r.CreatedBy).HasMaxLength(45);
        builder.Property(r => r.UpdatedBy).HasMaxLength(45);
    }
}
