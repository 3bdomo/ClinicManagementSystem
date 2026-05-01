using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Description)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_InvoiceItem_QuantityPositive",
            "Quantity > 0"
        ));

        builder.Property(item => item.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(item => item.ItemType)
            .HasConversion<string>();

    }
}
