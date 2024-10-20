using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace integration_platform.database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordTransferContent",
                columns: table => new
                {
                    RecordTransferContentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<byte[]>(type: "bytea", nullable: true),
                    RecCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 41, DateTimeKind.Utc).AddTicks(102)),
                    RecModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 41, DateTimeKind.Utc).AddTicks(356))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordTransferContent", x => x.RecordTransferContentId);
                });

            migrationBuilder.CreateTable(
                name: "RecordTransfer",
                columns: table => new
                {
                    RecordTransferId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResponseContentId = table.Column<long>(type: "bigint", nullable: true),
                    RequestContentId = table.Column<long>(type: "bigint", nullable: true),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    SourceId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusMessage = table.Column<string>(type: "text", nullable: true),
                    Target = table.Column<string>(type: "text", nullable: false),
                    TargetId = table.Column<string>(type: "text", nullable: true),
                    RecCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 40, DateTimeKind.Utc).AddTicks(50)),
                    RecModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 40, DateTimeKind.Utc).AddTicks(407))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordTransfer", x => x.RecordTransferId);
                    table.ForeignKey(
                        name: "FK_RecordTransfer_RecordTransferContent_RequestContentId",
                        column: x => x.RequestContentId,
                        principalTable: "RecordTransferContent",
                        principalColumn: "RecordTransferContentId");
                    table.ForeignKey(
                        name: "FK_RecordTransfer_RecordTransferContent_ResponseContentId",
                        column: x => x.ResponseContentId,
                        principalTable: "RecordTransferContent",
                        principalColumn: "RecordTransferContentId");
                });

            migrationBuilder.CreateTable(
                name: "TransformRecord",
                columns: table => new
                {
                    TransformRecordId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordType = table.Column<string>(type: "text", nullable: false),
                    EntityVersion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InRecordTransferId = table.Column<long>(type: "bigint", nullable: true),
                    OutRecordTransferId = table.Column<long>(type: "bigint", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: false),
                    SourceId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusMessage = table.Column<string>(type: "text", nullable: true),
                    Target = table.Column<string>(type: "text", nullable: false),
                    TargetId = table.Column<string>(type: "text", nullable: true),
                    RecCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 40, DateTimeKind.Utc).AddTicks(3030)),
                    RecModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 40, DateTimeKind.Utc).AddTicks(3365))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransformRecord", x => x.TransformRecordId);
                    table.ForeignKey(
                        name: "FK_TransformRecord_RecordTransfer_InRecordTransferId",
                        column: x => x.InRecordTransferId,
                        principalTable: "RecordTransfer",
                        principalColumn: "RecordTransferId");
                    table.ForeignKey(
                        name: "FK_TransformRecord_RecordTransfer_OutRecordTransferId",
                        column: x => x.OutRecordTransferId,
                        principalTable: "RecordTransfer",
                        principalColumn: "RecordTransferId");
                });

            migrationBuilder.CreateTable(
                name: "TransformRecordContent",
                columns: table => new
                {
                    TransformRecordContentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<byte[]>(type: "bytea", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    TransformRecordId = table.Column<long>(type: "bigint", nullable: true),
                    TransformRecordId1 = table.Column<long>(type: "bigint", nullable: true),
                    RecCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 41, DateTimeKind.Utc).AddTicks(1085)),
                    RecModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 10, 20, 15, 20, 57, 41, DateTimeKind.Utc).AddTicks(1375))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransformRecordContent", x => x.TransformRecordContentId);
                    table.ForeignKey(
                        name: "FK_TransformRecordContent_TransformRecord_TransformRecordId",
                        column: x => x.TransformRecordId,
                        principalTable: "TransformRecord",
                        principalColumn: "TransformRecordId");
                    table.ForeignKey(
                        name: "FK_TransformRecordContent_TransformRecord_TransformRecordId1",
                        column: x => x.TransformRecordId1,
                        principalTable: "TransformRecord",
                        principalColumn: "TransformRecordId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordTransfer_RequestContentId",
                table: "RecordTransfer",
                column: "RequestContentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordTransfer_ResponseContentId",
                table: "RecordTransfer",
                column: "ResponseContentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransformRecord_InRecordTransferId",
                table: "TransformRecord",
                column: "InRecordTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_TransformRecord_OutRecordTransferId",
                table: "TransformRecord",
                column: "OutRecordTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_TransformRecordContent_TransformRecordId",
                table: "TransformRecordContent",
                column: "TransformRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TransformRecordContent_TransformRecordId1",
                table: "TransformRecordContent",
                column: "TransformRecordId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransformRecordContent");

            migrationBuilder.DropTable(
                name: "TransformRecord");

            migrationBuilder.DropTable(
                name: "RecordTransfer");

            migrationBuilder.DropTable(
                name: "RecordTransferContent");
        }
    }
}
