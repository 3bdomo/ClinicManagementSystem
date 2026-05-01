using ClinicSystem.DAL.Models;
using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasDefaultValue(InvoiceStatus.Unpaid);

        builder.Property(i => i.PaidAt)
            .IsRequired(false);

        builder.HasIndex(i => i.AppointmentId)
            .IsUnique()
            .HasDatabaseName("UX_Invoice_AppointmentId");

        builder.HasMany(i => i.Items)
            .WithOne(item => item.Invoice)
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Property(i => i.CreatedBy).HasMaxLength(45);
        builder.Property(i => i.UpdatedBy).HasMaxLength(45);
    }
}
