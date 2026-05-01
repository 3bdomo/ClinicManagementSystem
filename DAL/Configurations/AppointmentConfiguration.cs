using ClinicSystem.DAL.Models;
using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

    
        builder.Property(a => a.AppointmentDate)
            .IsRequired();


        builder.Property(a => a.DurationMinutes)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Appointment_DurationPositive",
            "DurationMinutes > 0"
        ));

        builder.Property(a => a.AppointmentType)
            .HasConversion<string>();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasDefaultValue(AppointmentStatus.Waiting);

        builder.Property(a => a.Notes).HasMaxLength(500);

        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate })
            .HasDatabaseName("IX_Appointment_DoctorId_AppointmentDate");


        builder.HasOne(a => a.MedicalRecord)
            .WithOne(m => m.Appointment)
            .HasForeignKey<MedicalRecord>(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Invoice)
            .WithOne(i => i.Appointment)
            .HasForeignKey<Invoice>(i => i.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.CreatedBy).HasMaxLength(45);
        builder.Property(a => a.UpdatedBy).HasMaxLength(45);
    }
}
