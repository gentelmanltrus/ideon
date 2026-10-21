using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ideon.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCollaborationRequestUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CollaborationRequests_IdeaId",
                table: "CollaborationRequests");

            migrationBuilder.CreateIndex(
                name: "IX_CollaborationRequests_IdeaId_UserId",
                table: "CollaborationRequests",
                columns: new[] { "IdeaId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CollaborationRequests_IdeaId_UserId",
                table: "CollaborationRequests");

            migrationBuilder.CreateIndex(
                name: "IX_CollaborationRequests_IdeaId",
                table: "CollaborationRequests",
                column: "IdeaId");
        }
    }
}
