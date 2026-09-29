using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ideon.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseConstraintsAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Game Development" },
                    { 2, "Technology" },
                    { 3, "Education" },
                    { 4, "Entertainment" },
                    { 5, "Social" },
                    { 6, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId_IdeaId",
                table: "Ratings",
                columns: new[] { "UserId", "IdeaId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Ratings_Feasibility",
                table: "Ratings",
                sql: "\"Feasibility\" BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Ratings_Originality",
                table: "Ratings",
                sql: "\"Originality\" BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Ratings_Usefulness",
                table: "Ratings",
                sql: "\"Usefulness\" BETWEEN 1 AND 5");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_UserId_IdeaId",
                table: "Ratings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Ratings_Feasibility",
                table: "Ratings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Ratings_Originality",
                table: "Ratings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Ratings_Usefulness",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");
        }
    }
}
