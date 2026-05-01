using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Diagnosis)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.Property(m => m.VisitDate)
            .IsRequired();

        builder.Property(m => m.FollowUpDate)
            .IsRequired(false);

        builder.HasIndex(m => m.AppointmentId)
            .IsUnique()
            .HasDatabaseName("UX_MedicalRecord_AppointmentId");

        builder.HasMany(m => m.Attachments)
            .WithOne(a => a.MedicalRecord)
            .HasForeignKey(a => a.MedicalRecordId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasMany(m => m.Procedures)
            .WithOne(p => p.MedicalRecord)
            .HasForeignKey(p => p.MedicalRecordId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Property(m => m.CreatedBy).HasMaxLength(45);
        builder.Property(m => m.UpdatedBy).HasMaxLength(45);
    }
}
