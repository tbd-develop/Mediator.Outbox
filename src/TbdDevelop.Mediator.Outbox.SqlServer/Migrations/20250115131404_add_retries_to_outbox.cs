using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbdDevelop.Mediator.Outbox.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class add_retries_to_outbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Retries",
                table: "NotificationOutbox",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Retries",
                table: "NotificationOutbox");
        }
    }
}
