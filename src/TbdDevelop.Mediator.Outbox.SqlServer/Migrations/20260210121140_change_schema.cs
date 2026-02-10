using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TbdDevelop.Mediator.Outbox.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class change_schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "outbox");

            migrationBuilder.RenameTable(
                name: "NotificationOutbox.DeadLetterQueue",
                newName: "NotificationOutbox.DeadLetterQueue",
                newSchema: "outbox");

            migrationBuilder.RenameTable(
                name: "NotificationOutbox",
                newName: "NotificationOutbox",
                newSchema: "outbox");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "NotificationOutbox.DeadLetterQueue",
                schema: "outbox",
                newName: "NotificationOutbox.DeadLetterQueue");

            migrationBuilder.RenameTable(
                name: "NotificationOutbox",
                schema: "outbox",
                newName: "NotificationOutbox");
        }
    }
}
