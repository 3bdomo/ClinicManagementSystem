using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
{
    public void Configure(EntityTypeBuilder<Procedure> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MedicalRecordId)
            .IsRequired();

        builder.Property(p => p.PerformedAt)
            .IsRequired();

        builder.Property(p => p.DurationMinutes)
            .IsRequired(false);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.AfterCareNotes)
            .HasMaxLength(1000);

        builder.Property(p => p.Cost)
            .HasColumnType("decimal(18,2)");


        builder.Property(p => p.CreatedBy).HasMaxLength(45);
        builder.Property(p => p.UpdatedBy).HasMaxLength(45);
    }
}
