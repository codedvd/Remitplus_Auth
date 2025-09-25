using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Remitplus_Authentication.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    ApiKeyId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    ApiKeyHash = table.Column<string>(type: "text", nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    ApplicationUserUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserApiKeys_ApplicationUsers_ApplicationUserUser~",
                        column: x => x.ApplicationUserUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserApiKeys_ApplicationUserUserId",
                table: "ApplicationUserApiKeys",
                column: "ApplicationUserUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserApiKeys");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");
        }
    }
}
