using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.UserRole)
            .HasConversion<string>();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.HasOne(u => u.Doctor)
            .WithOne(d => d.ApplicationUser)
            .HasForeignKey<Doctor>(d => d.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(u => u.Patient)
            .WithOne(p => p.ApplicationUser)
            .HasForeignKey<Patient>(p => p.ApplicationUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull); 

        builder.HasOne(u => u.Receptionist)
            .WithOne(r => r.ApplicationUser)
            .HasForeignKey<Receptionist>(r => r.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
