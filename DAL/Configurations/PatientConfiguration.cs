using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.NationalId)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(p => p.NationalId)
            .IsUnique()
            .HasDatabaseName("UX_Patient_NationalId");

        builder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(p => p.Phone)
            .HasDatabaseName("IX_Patient_Phone");

        builder.Property(p => p.Gender)
            .HasConversion<string>();

        builder.Property(p => p.Address).HasMaxLength(250);
        builder.Property(p => p.BloodType).HasMaxLength(5);
        builder.Property(p => p.EmergencyContact).HasMaxLength(150);

        builder.Property(p => p.ApplicationUserId)
            .IsRequired(false);

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.DeletedBy).HasMaxLength(45);

        // IAuditable fields
        builder.Property(p => p.CreatedBy).HasMaxLength(45);
        builder.Property(p => p.UpdatedBy).HasMaxLength(45);

        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.MedicalRecords)
            .WithOne(m => m.Patient)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Invoices)
            .WithOne(i => i.Patient)
            .HasForeignKey(i => i.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
