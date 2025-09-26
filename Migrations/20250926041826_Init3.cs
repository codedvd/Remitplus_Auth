using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remitplus_Authentication.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserApiKeys_ApplicationUsers_ApplicationUserUser~",
                table: "ApplicationUserApiKeys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUsers",
                table: "ApplicationUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserApiKeys",
                table: "ApplicationUserApiKeys");

            migrationBuilder.RenameTable(
                name: "ApplicationUsers",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "ApplicationUserApiKeys",
                newName: "UserApiKeys");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserApiKeys_ApplicationUserUserId",
                table: "UserApiKeys",
                newName: "IX_UserApiKeys_ApplicationUserUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "UserApiKeys",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApiKeys",
                table: "UserApiKeys",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IPBlackLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IPAddress = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPBlackLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IpWhitelists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpWhitelists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IpWhitelists_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WhitelistedIpLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    ApiRoute = table.Column<string>(type: "text", nullable: false),
                    HttpMethod = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    RequestPayload = table.Column<string>(type: "text", nullable: true),
                    ResponsePayload = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhitelistedIpLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhitelistedIpLog_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IpWhitelists_ApplicationUserId",
                table: "IpWhitelists",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WhitelistedIpLog_ApplicationUserId",
                table: "WhitelistedIpLog",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApiKeys_Users_ApplicationUserUserId",
                table: "UserApiKeys",
                column: "ApplicationUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApiKeys_Users_ApplicationUserUserId",
                table: "UserApiKeys");

            migrationBuilder.DropTable(
                name: "IPBlackLists");

            migrationBuilder.DropTable(
                name: "IpWhitelists");

            migrationBuilder.DropTable(
                name: "WhitelistedIpLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApiKeys",
                table: "UserApiKeys");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "UserApiKeys");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "ApplicationUsers");

            migrationBuilder.RenameTable(
                name: "UserApiKeys",
                newName: "ApplicationUserApiKeys");

            migrationBuilder.RenameIndex(
                name: "IX_UserApiKeys_ApplicationUserUserId",
                table: "ApplicationUserApiKeys",
                newName: "IX_ApplicationUserApiKeys_ApplicationUserUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUsers",
                table: "ApplicationUsers",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserApiKeys",
                table: "ApplicationUserApiKeys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserApiKeys_ApplicationUsers_ApplicationUserUser~",
                table: "ApplicationUserApiKeys",
                column: "ApplicationUserUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "UserId");
        }
    }
}
