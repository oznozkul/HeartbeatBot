using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeartbeatBot.Job.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_App_ApplicationId",
                table: "OutboxMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_App",
                table: "App");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                newName: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "App",
                newName: "Apps");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_ApplicationId",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_ApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Apps",
                table: "Apps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessages_Apps_ApplicationId",
                table: "OutboxMessages",
                column: "ApplicationId",
                principalTable: "Apps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessages_Apps_ApplicationId",
                table: "OutboxMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Apps",
                table: "Apps");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "OutboxMessage");

            migrationBuilder.RenameTable(
                name: "Apps",
                newName: "App");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_ApplicationId",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_ApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_App",
                table: "App",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_App_ApplicationId",
                table: "OutboxMessage",
                column: "ApplicationId",
                principalTable: "App",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
