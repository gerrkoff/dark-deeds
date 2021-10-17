CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256) NULL,
    "NormalizedName" character varying(256) NULL,
    "ConcurrencyStamp" text NULL,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUsers" (
    "Id" text NOT NULL,
    "UserName" character varying(256) NULL,
    "NormalizedUserName" character varying(256) NULL,
    "Email" character varying(256) NULL,
    "NormalizedEmail" character varying(256) NULL,
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text NULL,
    "SecurityStamp" text NULL,
    "ConcurrencyStamp" text NULL,
    "PhoneNumber" text NULL,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone NULL,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    "DisplayName" text NULL,
    "TelegramChatKey" text NULL,
    "TelegramChatId" integer NOT NULL,
    "TelegramTimeAdjustment" integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" serial NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" serial NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text NULL,
    "UserId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Settings" (
    "Id" serial NOT NULL,
    "ShowCompleted" boolean NOT NULL,
    "UserId" text NOT NULL,
    CONSTRAINT "PK_Settings" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Settings_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Tasks" (
    "Id" serial NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "Title" text NULL,
    "Order" integer NOT NULL,
    "DateTime" timestamp without time zone NULL,
    "TimeType" integer NOT NULL,
    "IsCompleted" boolean NOT NULL,
    "IsProbable" boolean NOT NULL,
    "Version" integer NOT NULL,
    "UserId" text NOT NULL,
    CONSTRAINT "PK_Tasks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tasks_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE INDEX "IX_Settings_UserId" ON "Settings" ("UserId");

CREATE INDEX "IX_Tasks_UserId" ON "Tasks" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190619201215_N000_Init', '5.0.5');

COMMIT;

START TRANSACTION;

ALTER TABLE "Tasks" RENAME COLUMN "DateTime" TO "Date";

ALTER TABLE "Tasks" ADD "Time" integer NULL;


UPDATE "Tasks" SET "Date" = "Date" + interval '3 hour'; -- just because I know all my tasks was created in UTC+3h timezone

UPDATE "Tasks"
SET "Time" = date_part('hour', "Date") * 60 + date_part('minute', "Date"),
    "Date" = date_trunc('day', "Date"),
    "TimeType" = 0
WHERE "TimeType" = 1;
            

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190914163948_N001_Add_Time_to_TaskEntity', '5.0.5');

COMMIT;

START TRANSACTION;

ALTER TABLE "Tasks" RENAME COLUMN "TimeType" TO "Type";


UPDATE "Tasks"
SET "Type" = 1
WHERE "Type" = 2;
            

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190915183000_N002_Rename_TimeType_to_Type_in_TaskEntity', '5.0.5');

COMMIT;

START TRANSACTION;

CREATE TABLE "PlannedRecurrences" (
    "Id" serial NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "Task" text NULL,
    "StartDate" timestamp without time zone NOT NULL,
    "EndDate" timestamp without time zone NULL,
    "EveryNthDay" integer NULL,
    "EveryMonthDay" text NULL,
    "EveryWeekday" integer NULL,
    "UserId" text NOT NULL,
    CONSTRAINT "PK_PlannedRecurrences" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PlannedRecurrences_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Recurrences" (
    "Id" serial NOT NULL,
    "DateTime" timestamp without time zone NOT NULL,
    "PlannedRecurrenceId" integer NOT NULL,
    "TaskId" integer NOT NULL,
    CONSTRAINT "PK_Recurrences" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Recurrences_PlannedRecurrences_PlannedRecurrenceId" FOREIGN KEY ("PlannedRecurrenceId") REFERENCES "PlannedRecurrences" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Recurrences_Tasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "Tasks" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_PlannedRecurrences_UserId" ON "PlannedRecurrences" ("UserId");

CREATE INDEX "IX_Recurrences_PlannedRecurrenceId" ON "Recurrences" ("PlannedRecurrenceId");

CREATE INDEX "IX_Recurrences_TaskId" ON "Recurrences" ("TaskId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20191027214301_N003_Add_PlannedRecurences_And_Recurrences', '5.0.5');

COMMIT;

START TRANSACTION;

ALTER TABLE "Tasks" ADD "Uid" text NULL DEFAULT '';

UPDATE "Tasks" SET "Uid" = "Id";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210928165654_N004_Add_Uid_to_Tasks', '5.0.5');

COMMIT;


