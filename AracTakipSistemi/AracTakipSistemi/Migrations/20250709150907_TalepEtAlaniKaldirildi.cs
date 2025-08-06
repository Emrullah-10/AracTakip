using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class TalepEtAlaniKaldirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TalepEt",
                table: "Randevu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TalepEt",
                table: "Randevu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
