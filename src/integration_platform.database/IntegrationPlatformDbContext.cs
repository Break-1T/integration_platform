using integration_platform.database.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace integration_platform.database;

public class IntegrationPlatformDbContext : DbContext
{
    public IntegrationPlatformDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<RecordTransferContent> RecordTransferContents { get; set; }
    public DbSet<RecordTransfer> RecordTransfers { get; set; }
    public DbSet<TransformRecordContent> TransformRecordContents { get; set; }
    public DbSet<TransformRecord> TransformRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecordTransfer>(entity =>
        {
            entity.ToTable("RecordTransfer");

            entity.Property(e => e.RecordTransferId).ValueGeneratedOnAdd();
            entity.Property(e => e.RecordType).IsRequired();
            entity.Property(e => e.Source).IsRequired();
            entity.Property(e => e.SourceId);
            entity.Property(e => e.Status);
            entity.Property(e => e.StatusMessage);
            entity.Property(e => e.Target).IsRequired();
            entity.Property(e => e.TargetId);
            entity.Property(e => e.RecCreated).HasDefaultValueSql("now() at time zone 'utc'");
            entity.Property(e => e.RecModified).HasDefaultValueSql("now() at time zone 'utc'");

            entity.HasKey(e => e.RecordTransferId);

            entity.HasOne(recTransfer => recTransfer.RequestContent)
                .WithMany()
                .HasForeignKey(recTransfer => recTransfer.RequestContentId);

            entity.HasOne(recTransfer => recTransfer.ResponseContent)
                .WithMany()
                .HasForeignKey(recTransfer => recTransfer.ResponseContentId);
        });

        modelBuilder.Entity<TransformRecord>(entity =>
        {
            entity.ToTable("TransformRecord");

            entity.Property(e => e.TransformRecordId).ValueGeneratedOnAdd();
            entity.Property(e => e.RecordType).IsRequired();
            entity.Property(e => e.Source).IsRequired();
            entity.Property(e => e.SourceId);
            entity.Property(e => e.Status);
            entity.Property(e => e.StatusMessage);
            entity.Property(e => e.Target).IsRequired();
            entity.Property(e => e.EntityVersion).IsRequired();
            entity.Property(e => e.RecCreated).HasDefaultValueSql("now() at time zone 'utc'");
            entity.Property(e => e.RecModified).HasDefaultValueSql("now() at time zone 'utc'");

            entity.HasKey(e => e.TransformRecordId);

            entity.HasOne(transformRecord => transformRecord.InRecordTransfer)
                .WithMany()
                .HasForeignKey(transformRecord => transformRecord.InRecordTransferId);

            entity.HasOne(transformRecord => transformRecord.OutRecordTransfer)
                .WithMany()
                .HasForeignKey(transformRecord => transformRecord.OutRecordTransferId);

            entity.HasMany(transformRecord => transformRecord.ContentList)
                .WithOne()
                .HasForeignKey(recordContent => recordContent.TransformRecordId);
        });

        modelBuilder.Entity<RecordTransferContent>(entity =>
        {
            entity.ToTable("RecordTransferContent");

            entity.Property(e => e.RecordTransferContentId).ValueGeneratedOnAdd();
            entity.Property(e => e.Content);
            entity.Property(e => e.RecCreated).HasDefaultValueSql("now() at time zone 'utc'");
            entity.Property(e => e.RecModified).HasDefaultValueSql("now() at time zone 'utc'");

            entity.HasKey(e => e.RecordTransferContentId);
        });

        modelBuilder.Entity<TransformRecordContent>(entity =>
        {
            entity.ToTable("TransformRecordContent");

            entity.Property(e => e.TransformRecordContentId).ValueGeneratedOnAdd();
            entity.Property(e => e.Content);
            entity.Property(e => e.ContentType).IsRequired();
            entity.Property(e => e.RecCreated).HasDefaultValueSql("now() at time zone 'utc'");
            entity.Property(e => e.RecModified).HasDefaultValueSql("now() at time zone 'utc'");

            entity.HasKey(e => e.TransformRecordContentId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
