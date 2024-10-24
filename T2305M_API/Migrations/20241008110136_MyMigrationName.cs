using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T2305M_API.Migrations
{
    /// <inheritdoc />
    public partial class MyMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Creator_CreatorId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_CreatorId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_CreatorId",
                table: "Event",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Creator_CreatorId",
                table: "Event",
                column: "CreatorId",
                principalTable: "Creator",
                principalColumn: "CreatorId");
        }
    }
}
