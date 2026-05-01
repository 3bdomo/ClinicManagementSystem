using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class ProcedureTypeConfiguration : IEntityTypeConfiguration<ProcedureType>
{
    public void Configure(EntityTypeBuilder<ProcedureType> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(pt => pt.Name)
            .IsUnique()
            .HasDatabaseName("UX_ProcedureType_Name");

        builder.Property(pt => pt.Description)
            .HasMaxLength(500);

        builder.Property(pt => pt.DefaultCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(pt => pt.IsActive)
            .HasDefaultValue(true);


        builder.HasMany(pt => pt.Procedures)
            .WithOne(p => p.ProcedureType)
            .HasForeignKey(p => p.ProcedureTypeId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}
