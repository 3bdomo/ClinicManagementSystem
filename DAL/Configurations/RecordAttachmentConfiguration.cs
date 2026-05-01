using ClinicSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.DAL.Configurations;

public class RecordAttachmentConfiguration : IEntityTypeConfiguration<RecordAttachment>
{
    public void Configure(EntityTypeBuilder<RecordAttachment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.FileType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.UploadedAt)
            .IsRequired();

        builder.Property(a => a.UploadedBy)
            .HasMaxLength(45);

    }
}
