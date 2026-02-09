CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Clients" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Industry" text,
    "ContactEmail" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" text,
    "IsDeleted" boolean NOT NULL,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Clients" PRIMARY KEY ("Id")
);

CREATE TABLE "Staffs" (
    "Id" uuid NOT NULL,
    "FullName" character varying(200),
    "Username" text,
    "PasswordHash" text,
    "Department" integer NOT NULL,
    "Role" integer NOT NULL,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" text,
    "IsDeleted" boolean NOT NULL,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Staffs" PRIMARY KEY ("Id")
);

CREATE TABLE "SystemPhases" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" text,
    "DefaultSequence" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" text,
    "IsDeleted" boolean NOT NULL,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_SystemPhases" PRIMARY KEY ("Id")
);

CREATE TABLE "Projects" (
    "Id" uuid NOT NULL,
    "ProjectCode" text,
    "ParentProjectId" uuid,
    "Name" text NOT NULL,
    "ClientId" uuid NOT NULL,
    "Description" text NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "Deadline" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "Priority" integer NOT NULL,
    "ManagerId" uuid NOT NULL,
    "InternalBudget" numeric(18,2),
    "ContactPIC" text,
    "CustomerBudget" numeric(18,2),
    "CalculationConfigJson" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" text,
    "IsDeleted" boolean NOT NULL,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Projects" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Projects_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Projects_Projects_ParentProjectId" FOREIGN KEY ("ParentProjectId") REFERENCES "Projects" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Projects_Staffs_ManagerId" FOREIGN KEY ("ManagerId") REFERENCES "Staffs" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ProjectSystemPhaseConfigs" (
    "Id" uuid NOT NULL,
    "ProjectId" uuid NOT NULL,
    "SystemPhaseId" uuid NOT NULL,
    "Sequence" integer NOT NULL,
    "IsEnabled" boolean NOT NULL,
    "CustomPhaseName" character varying(100),
    "CustomDescription" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" text,
    "IsDeleted" boolean NOT NULL,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_ProjectSystemPhaseConfigs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProjectSystemPhaseConfigs_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProjectSystemPhaseConfigs_SystemPhases_SystemPhaseId" FOREIGN KEY ("SystemPhaseId") REFERENCES "SystemPhases" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "PhaseTasks" (
    "Id" uuid NOT NULL,
    "ProjectId" uuid NOT NULL,
    "ProjectSystemPhaseConfigId" uuid NOT NULL,
    "Description" text NOT NULL,
    "AssignedStaffId" uuid NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "DueDate" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "Priority" integer NOT NULL,
    "ProcessRate" smallint NOT NULL DEFAULT 0,
    "CompletedAt" timestamp with time zone,
    "EstimatedHours" numeric(10,2) NOT NULL DEFAULT 0.0,
    "InternalBudget" numeric,
    "CustomerBudget" numeric,
    "ParentPhaseTaskId" uuid,
    "StaffId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" text,
    "IsDeleted" boolean NOT NULL,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_PhaseTasks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PhaseTasks_PhaseTasks_ParentPhaseTaskId" FOREIGN KEY ("ParentPhaseTaskId") REFERENCES "PhaseTasks" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PhaseTasks_ProjectSystemPhaseConfigs_ProjectSystemPhaseConf~" FOREIGN KEY ("ProjectSystemPhaseConfigId") REFERENCES "ProjectSystemPhaseConfigs" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PhaseTasks_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PhaseTasks_Staffs_AssignedStaffId" FOREIGN KEY ("AssignedStaffId") REFERENCES "Staffs" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PhaseTasks_Staffs_StaffId" FOREIGN KEY ("StaffId") REFERENCES "Staffs" ("Id")
);

CREATE TABLE "WorkLogs" (
    "Id" uuid NOT NULL,
    "ProjectId" uuid NOT NULL,
    "PhaseTaskId" uuid NOT NULL,
    "OperatorId" uuid NOT NULL,
    "V1" numeric(18,2),
    "V2" numeric(18,2),
    "V3" numeric(18,2),
    "V4" numeric(18,2),
    "V5" numeric(18,2),
    "V6" numeric(18,2),
    "V7" numeric(18,2),
    "V8" numeric(18,2),
    "V9" numeric(18,2),
    "V10" numeric(18,2),
    "Type" integer NOT NULL,
    "ValueMain" numeric(18,2) NOT NULL,
    "ValueSub" numeric(18,2),
    "Status" integer NOT NULL,
    "DynamicDataJson" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" text,
    "IsDeleted" boolean NOT NULL,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_WorkLogs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_WorkLogs_PhaseTasks_PhaseTaskId" FOREIGN KEY ("PhaseTaskId") REFERENCES "PhaseTasks" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_WorkLogs_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_WorkLogs_Staffs_OperatorId" FOREIGN KEY ("OperatorId") REFERENCES "Staffs" ("Id") ON DELETE RESTRICT
);

INSERT INTO "Clients" ("Id", "ContactEmail", "CreatedAt", "CreatedBy", "Industry", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy")
VALUES ('188cd869-567e-4cd2-870a-48bdb04af5cd', 'contact@samsung.com', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'Electronics', FALSE, 'Samsung Vina', NULL, NULL);

INSERT INTO "Staffs" ("Id", "CreatedAt", "CreatedBy", "Department", "FullName", "IsDeleted", "PasswordHash", "Role", "Status", "UpdatedAt", "UpdatedBy", "Username")
VALUES ('1b8c3dbf-63bb-4207-b108-9b28706185a7', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 1, 'Huy Dang', FALSE, 'AQAAAAEAACcQAAAAE...', 1, 0, NULL, NULL, 'huydang.admin');

INSERT INTO "SystemPhases" ("Id", "CreatedAt", "CreatedBy", "DefaultSequence", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy")
VALUES ('11111111-1111-1111-1111-111111111111', TIMESTAMPTZ '2026-01-30T02:38:38Z', NULL, 1, 'Project initialization and setup', TRUE, FALSE, 'Initialize', NULL, NULL);
INSERT INTO "SystemPhases" ("Id", "CreatedAt", "CreatedBy", "DefaultSequence", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy")
VALUES ('22222222-2222-2222-2222-222222222222', TIMESTAMPTZ '2026-01-30T02:38:38Z', NULL, 2, 'Design and engineering phase', TRUE, FALSE, 'Engineering', NULL, NULL);
INSERT INTO "SystemPhases" ("Id", "CreatedAt", "CreatedBy", "DefaultSequence", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy")
VALUES ('33333333-3333-3333-3333-333333333333', TIMESTAMPTZ '2026-01-30T02:38:38Z', NULL, 3, 'Manufacturing and fabrication', TRUE, FALSE, 'Fabrication', NULL, NULL);
INSERT INTO "SystemPhases" ("Id", "CreatedAt", "CreatedBy", "DefaultSequence", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy")
VALUES ('44444444-4444-4444-4444-444444444444', TIMESTAMPTZ '2026-01-30T02:38:38Z', NULL, 4, 'CNC cutting operations', TRUE, FALSE, 'CNC Cutting', NULL, NULL);
INSERT INTO "SystemPhases" ("Id", "CreatedAt", "CreatedBy", "DefaultSequence", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy")
VALUES ('55555555-5555-5555-5555-555555555555', TIMESTAMPTZ '2026-01-30T02:38:38Z', NULL, 5, 'Product assembly phase', TRUE, FALSE, 'Assembly', NULL, NULL);

CREATE INDEX "IX_PhaseTasks_AssignedStaffId" ON "PhaseTasks" ("AssignedStaffId");

CREATE INDEX "IX_PhaseTasks_ParentPhaseTaskId" ON "PhaseTasks" ("ParentPhaseTaskId");

CREATE INDEX "IX_PhaseTasks_ProjectId" ON "PhaseTasks" ("ProjectId");

CREATE INDEX "IX_PhaseTasks_ProjectSystemPhaseConfigId" ON "PhaseTasks" ("ProjectSystemPhaseConfigId");

CREATE INDEX "IX_PhaseTasks_StaffId" ON "PhaseTasks" ("StaffId");

CREATE INDEX "IX_Projects_ClientId" ON "Projects" ("ClientId");

CREATE INDEX "IX_Projects_ManagerId" ON "Projects" ("ManagerId");

CREATE INDEX "IX_Projects_ParentProjectId" ON "Projects" ("ParentProjectId");

CREATE UNIQUE INDEX "IX_Projects_ProjectCode" ON "Projects" ("ProjectCode");

CREATE UNIQUE INDEX "IX_ProjectSystemPhaseConfigs_ProjectId_SystemPhaseId" ON "ProjectSystemPhaseConfigs" ("ProjectId", "SystemPhaseId");

CREATE INDEX "IX_ProjectSystemPhaseConfigs_SystemPhaseId" ON "ProjectSystemPhaseConfigs" ("SystemPhaseId");

CREATE UNIQUE INDEX "IX_SystemPhases_Name" ON "SystemPhases" ("Name");

CREATE INDEX "IX_WorkLogs_OperatorId" ON "WorkLogs" ("OperatorId");

CREATE INDEX "IX_WorkLogs_PhaseTaskId" ON "WorkLogs" ("PhaseTaskId");

CREATE INDEX "IX_WorkLogs_ProjectId" ON "WorkLogs" ("ProjectId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260209110012_InitialCreate', '10.0.2');

COMMIT;

