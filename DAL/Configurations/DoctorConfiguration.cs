using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FullName)
            .IsRequired()
            .HasMaxLength(100);


        builder.Property(d => d.ApplicationUserId)
            .IsRequired();

        builder.Property(d => d.ConsultationFee)
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.Specialization)
            .HasConversion<string>();

        builder.Property(d => d.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(d => d.Bio)
            .HasMaxLength(1000);


        builder.HasMany(d => d.DoctorSchedules)
            .WithOne(s => s.Doctor)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasMany(d => d.MedicalRecords)
            .WithOne(m => m.Doctor)
            .HasForeignKey(m => m.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
