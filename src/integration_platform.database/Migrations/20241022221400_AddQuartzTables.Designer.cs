﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using integration_platform.database;

#nullable disable

namespace integration_platform.database.Migrations
{
    [DbContext(typeof(IntegrationPlatformDbContext))]
    [Migration("20241022221400_AddQuartzTables")]
    partial class AddQuartzTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("integration_platform.database.Models.RecordTransfer", b =>
                {
                    b.Property<long?>("RecordTransferId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("RecordTransferId"));

                    b.Property<DateTime>("RecCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime>("RecModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("RecordType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("RequestContentId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ResponseContentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SourceId")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("StatusMessage")
                        .HasColumnType("text");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TargetId")
                        .HasColumnType("text");

                    b.HasKey("RecordTransferId");

                    b.HasIndex("RequestContentId");

                    b.HasIndex("ResponseContentId");

                    b.ToTable("RecordTransfer", (string)null);
                });

            modelBuilder.Entity("integration_platform.database.Models.RecordTransferContent", b =>
                {
                    b.Property<long?>("RecordTransferContentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("RecordTransferContentId"));

                    b.Property<byte[]>("Content")
                        .HasColumnType("bytea");

                    b.Property<DateTime>("RecCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime>("RecModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.HasKey("RecordTransferContentId");

                    b.ToTable("RecordTransferContent", (string)null);
                });

            modelBuilder.Entity("integration_platform.database.Models.TransformRecord", b =>
                {
                    b.Property<long?>("TransformRecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("TransformRecordId"));

                    b.Property<DateTime>("EntityVersion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("InRecordTransferId")
                        .HasColumnType("bigint");

                    b.Property<long?>("OutRecordTransferId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("RecCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime>("RecModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("RecordType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SourceId")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("StatusMessage")
                        .HasColumnType("text");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TargetId")
                        .HasColumnType("text");

                    b.HasKey("TransformRecordId");

                    b.HasIndex("InRecordTransferId");

                    b.HasIndex("OutRecordTransferId");

                    b.ToTable("TransformRecord", (string)null);
                });

            modelBuilder.Entity("integration_platform.database.Models.TransformRecordContent", b =>
                {
                    b.Property<long?>("TransformRecordContentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("TransformRecordContentId"));

                    b.Property<byte[]>("Content")
                        .HasColumnType("bytea");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RecCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<DateTime>("RecModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<long?>("TransformRecordId")
                        .HasColumnType("bigint");

                    b.Property<long?>("TransformRecordId1")
                        .HasColumnType("bigint");

                    b.HasKey("TransformRecordContentId");

                    b.HasIndex("TransformRecordId");

                    b.HasIndex("TransformRecordId1");

                    b.ToTable("TransformRecordContent", (string)null);
                });

            modelBuilder.Entity("integration_platform.database.Models.RecordTransfer", b =>
                {
                    b.HasOne("integration_platform.database.Models.RecordTransferContent", "RequestContent")
                        .WithMany()
                        .HasForeignKey("RequestContentId");

                    b.HasOne("integration_platform.database.Models.RecordTransferContent", "ResponseContent")
                        .WithMany()
                        .HasForeignKey("ResponseContentId");

                    b.Navigation("RequestContent");

                    b.Navigation("ResponseContent");
                });

            modelBuilder.Entity("integration_platform.database.Models.TransformRecord", b =>
                {
                    b.HasOne("integration_platform.database.Models.RecordTransfer", "InRecordTransfer")
                        .WithMany()
                        .HasForeignKey("InRecordTransferId");

                    b.HasOne("integration_platform.database.Models.RecordTransfer", "OutRecordTransfer")
                        .WithMany()
                        .HasForeignKey("OutRecordTransferId");

                    b.Navigation("InRecordTransfer");

                    b.Navigation("OutRecordTransfer");
                });

            modelBuilder.Entity("integration_platform.database.Models.TransformRecordContent", b =>
                {
                    b.HasOne("integration_platform.database.Models.TransformRecord", null)
                        .WithMany("ContentList")
                        .HasForeignKey("TransformRecordId");

                    b.HasOne("integration_platform.database.Models.TransformRecord", "TransformRecord")
                        .WithMany()
                        .HasForeignKey("TransformRecordId1");

                    b.Navigation("TransformRecord");
                });

            modelBuilder.Entity("integration_platform.database.Models.TransformRecord", b =>
                {
                    b.Navigation("ContentList");
                });
#pragma warning restore 612, 618
        }
    }
}
