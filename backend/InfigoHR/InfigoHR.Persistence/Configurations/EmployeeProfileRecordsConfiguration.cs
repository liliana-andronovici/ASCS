using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class EmployeeProfileRecordsConfiguration :
    IEntityTypeConfiguration<EmployeeDocumentEntity>,
    IEntityTypeConfiguration<EmployeeHistoryEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeDocumentEntity> builder)
    {
        builder.ToTable("EmployeeDocuments");
        builder.HasKey(document => document.Id);
        builder.Property(document => document.Name).HasMaxLength(200).IsRequired();
        builder.Property(document => document.FileName).HasMaxLength(255).IsRequired();
        builder.Property(document => document.ContentType).HasMaxLength(150).IsRequired();
        builder.HasIndex(document => new { document.EmployeeId, document.Id });
        builder.HasOne<EmployeeEntity>().WithMany().HasForeignKey(document => document.EmployeeId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<EmployeeHistoryEntity> builder)
    {
        builder.ToTable("EmployeeHistory");
        builder.HasKey(history => history.Id);
        builder.Property(history => history.ChangedFields).IsRequired();
        builder.HasIndex(history => new { history.EmployeeId, history.Id });
        builder.HasOne<EmployeeEntity>().WithMany().HasForeignKey(history => history.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<EmployeeEntity>().WithMany().HasForeignKey(history => history.ActorEmployeeId).OnDelete(DeleteBehavior.SetNull);
    }
}
