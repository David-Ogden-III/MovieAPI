using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserGenreRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "genres",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_genres_UserId",
                table: "genres",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_genres_AspNetUsers_UserId",
                table: "genres",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_genres_AspNetUsers_UserId",
                table: "genres");

            migrationBuilder.DropIndex(
                name: "IX_genres_UserId",
                table: "genres");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "genres");
        }
    }
}
