using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarinLend.Data.Migrations
{
    /// <inheritdoc />
    public partial class RejectionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RejectionReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GetDate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectionReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepRejectionReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RejectionReasonId = table.Column<int>(type: "int", nullable: false),
                    WorkFlowStepId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GetDate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowStepRejectionReasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlowStepRejectionReasons_RejectionReasons_RejectionReasonId",
                        column: x => x.RejectionReasonId,
                        principalTable: "RejectionReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkFlowStepRejectionReasons_WorkFlowSteps_WorkFlowStepId",
                        column: x => x.WorkFlowStepId,
                        principalTable: "WorkFlowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestFacilityWorkFlowStepId = table.Column<int>(type: "int", nullable: false),
                    WorkFlowStepRejectionReasonId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GetDate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons_RequestFacilityWorkFlowSteps_RequestFacilityWorkFlowStepId",
                        column: x => x.RequestFacilityWorkFlowStepId,
                        principalTable: "RequestFacilityWorkFlowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons_WorkFlowStepRejectionReasons_WorkFlowStepRejectionReasonId",
                        column: x => x.WorkFlowStepRejectionReasonId,
                        principalTable: "WorkFlowStepRejectionReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons_RequestFacilityWorkFlowStepId",
                table: "RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons",
                column: "RequestFacilityWorkFlowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons_WorkFlowStepRejectionReasonId",
                table: "RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons",
                column: "WorkFlowStepRejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepRejectionReasons_RejectionReasonId",
                table: "WorkFlowStepRejectionReasons",
                column: "RejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepRejectionReasons_WorkFlowStepId",
                table: "WorkFlowStepRejectionReasons",
                column: "WorkFlowStepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons");

            migrationBuilder.DropTable(
                name: "WorkFlowStepRejectionReasons");

            migrationBuilder.DropTable(
                name: "RejectionReasons");
        }
    }
}
