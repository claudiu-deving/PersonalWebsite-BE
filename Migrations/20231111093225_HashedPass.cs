using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ccsflowserver.Migrations
{
    /// <inheritdoc />
    public partial class HashedPass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Logins");

            migrationBuilder.AddColumn<byte[]>(
                name: "PassHash",
                table: "Logins",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PassSalt",
                table: "Logins",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassHash",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "PassSalt",
                table: "Logins");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Logins",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
