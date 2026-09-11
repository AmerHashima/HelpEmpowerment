using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpEmpowermentApi.Migrations
{
    /// <inheritdoc />
    public partial class paymentandrevenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_AppLookupD_RoleLookupId",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "RoleLookupId",
                table: "users",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_users_RoleLookupId",
                table: "users",
                newName: "IX_users_RoleId");

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM roles WHERE LOWER(Name) = 'admin' AND IsDeleted = 0)
                    INSERT INTO roles (Oid, Name, Description, IsActive, CreatedAt, IsDeleted)
                    VALUES ('A1000000-0000-0000-0000-000000000001', 'Admin', 'Application administrator', 1, SYSUTCDATETIME(), 0);

                IF NOT EXISTS (SELECT 1 FROM roles WHERE LOWER(Name) = 'trainer' AND IsDeleted = 0)
                    INSERT INTO roles (Oid, Name, Description, IsActive, CreatedAt, IsDeleted)
                    VALUES ('A1000000-0000-0000-0000-000000000002', 'Trainer', 'Course trainer', 1, SYSUTCDATETIME(), 0);

                UPDATE users
                SET RoleId = CASE
                    WHEN RoleId = '55555555-5555-5555-5555-555555555501'
                        THEN (SELECT TOP 1 Oid FROM roles WHERE LOWER(Name) = 'admin' AND IsDeleted = 0)
                    WHEN RoleId = '55555555-5555-5555-5555-555555555502'
                        THEN (SELECT TOP 1 Oid FROM roles WHERE LOWER(Name) = 'trainer' AND IsDeleted = 0)
                    ELSE NULL
                END;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Oid",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_RoleId",
                table: "users");

            migrationBuilder.Sql("""
                UPDATE users
                SET RoleId = CASE
                    WHEN RoleId = (SELECT TOP 1 Oid FROM roles WHERE LOWER(Name) = 'admin' AND IsDeleted = 0)
                        THEN '55555555-5555-5555-5555-555555555501'
                    WHEN RoleId = (SELECT TOP 1 Oid FROM roles WHERE LOWER(Name) = 'trainer' AND IsDeleted = 0)
                        THEN '55555555-5555-5555-5555-555555555502'
                    ELSE NULL
                END;
                """);

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "users",
                newName: "RoleLookupId");

            migrationBuilder.RenameIndex(
                name: "IX_users_RoleId",
                table: "users",
                newName: "IX_users_RoleLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_AppLookupD_RoleLookupId",
                table: "users",
                column: "RoleLookupId",
                principalTable: "AppLookupD",
                principalColumn: "Oid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
