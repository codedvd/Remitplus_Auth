using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Remitplus_Authentication.Migrations
{
    /// <inheritdoc />
    public partial class Init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "IpWhitelists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "IpWhitelists",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
