using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class RandevuOnayDurumuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnayDurumu",
                table: "Randevu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RedSebebi",
                table: "Randevu",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnayDurumu",
                table: "Randevu");

            migrationBuilder.DropColumn(
                name: "RedSebebi",
                table: "Randevu");
        }
    }
}
