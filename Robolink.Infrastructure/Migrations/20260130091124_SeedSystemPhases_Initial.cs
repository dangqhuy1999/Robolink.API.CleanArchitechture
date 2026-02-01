using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Robolink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSystemPhases_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemPhases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultSequence = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPhases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParentProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InternalBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ContactPIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CalculationConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Projects_ParentProjectId",
                        column: x => x.ParentProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Staffs_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Staffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSystemPhaseConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemPhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CustomPhaseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSystemPhaseConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSystemPhaseConfigs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSystemPhaseConfigs_SystemPhases_SystemPhaseId",
                        column: x => x.SystemPhaseId,
                        principalTable: "SystemPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhaseTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentPhaseTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectSystemPhaseConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessRate = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhaseTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhaseTasks_PhaseTasks_ParentPhaseTaskId",
                        column: x => x.ParentPhaseTaskId,
                        principalTable: "PhaseTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhaseTasks_ProjectSystemPhaseConfigs_ProjectSystemPhaseConfigId",
                        column: x => x.ProjectSystemPhaseConfigId,
                        principalTable: "ProjectSystemPhaseConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhaseTasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhaseTasks_Staffs_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhaseTasks_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    V1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V6 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V7 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V8 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V9 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    V10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ValueMain = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueSub = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DynamicDataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_PhaseTasks_PhaseTaskId",
                        column: x => x.PhaseTaskId,
                        principalTable: "PhaseTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Staffs_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Staffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SystemPhases",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DefaultSequence", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 30, 2, 38, 38, 0, DateTimeKind.Utc), null, 1, "Project initialization and setup", true, false, "Initialize", null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 30, 2, 38, 38, 0, DateTimeKind.Utc), null, 2, "Design and engineering phase", true, false, "Engineering", null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 30, 2, 38, 38, 0, DateTimeKind.Utc), null, 3, "Manufacturing and fabrication", true, false, "Fabrication", null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 1, 30, 2, 38, 38, 0, DateTimeKind.Utc), null, 4, "CNC cutting operations", true, false, "CNC Cutting", null, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 1, 30, 2, 38, 38, 0, DateTimeKind.Utc), null, 5, "Product assembly phase", true, false, "Assembly", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhaseTasks_AssignedStaffId",
                table: "PhaseTasks",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseTasks_ParentPhaseTaskId",
                table: "PhaseTasks",
                column: "ParentPhaseTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseTasks_ProjectId",
                table: "PhaseTasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseTasks_ProjectSystemPhaseConfigId",
                table: "PhaseTasks",
                column: "ProjectSystemPhaseConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseTasks_StaffId",
                table: "PhaseTasks",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ManagerId",
                table: "Projects",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ParentProjectId",
                table: "Projects",
                column: "ParentProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCode",
                table: "Projects",
                column: "ProjectCode",
                unique: true,
                filter: "[ProjectCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSystemPhaseConfigs_ProjectId_SystemPhaseId",
                table: "ProjectSystemPhaseConfigs",
                columns: new[] { "ProjectId", "SystemPhaseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSystemPhaseConfigs_SystemPhaseId",
                table: "ProjectSystemPhaseConfigs",
                column: "SystemPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPhases_Name",
                table: "SystemPhases",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_OperatorId",
                table: "WorkLogs",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_PhaseTaskId",
                table: "WorkLogs",
                column: "PhaseTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ProjectId",
                table: "WorkLogs",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "PhaseTasks");

            migrationBuilder.DropTable(
                name: "ProjectSystemPhaseConfigs");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "SystemPhases");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Staffs");
        }
    }
}
