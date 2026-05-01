using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ScheduleType)
            .HasConversion<string>();

        builder.Property(s => s.DayOfWeek)
            .HasConversion<int?>()  
            .IsRequired(false);

        builder.Property(s => s.SpecificDate)
            .IsRequired(false);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.SlotMinutes)
            .HasDefaultValue(30);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.HasIndex(s => new { s.DoctorId, s.DayOfWeek, s.ScheduleType })
            .HasDatabaseName("IX_DoctorSchedule_DoctorId_DayOfWeek_ScheduleType");

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_DoctorSchedule_DayOrDate",
            "(DayOfWeek IS NOT NULL AND SpecificDate IS NULL) OR (DayOfWeek IS NULL AND SpecificDate IS NOT NULL)"
        ));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_DoctorSchedule_StartBeforeEnd",
            "StartTime < EndTime"
        ));

        builder.HasMany(s => s.Appointments)
            .WithOne(a => a.DoctorSchedule)
            .HasForeignKey(a => a.DoctorScheduleId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.CreatedBy).HasMaxLength(45);
        builder.Property(s => s.UpdatedBy).HasMaxLength(45);
    }
}
