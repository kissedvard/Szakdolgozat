using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OkosBufeWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isFavorite",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isFavorite",
                table: "Orders");
        }
    }
}
