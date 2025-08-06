using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class RandevuTablosuGuncelle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Durum",
                table: "Randevu",
                newName: "TalepNedeni");

            migrationBuilder.AddColumn<int>(
                name: "KisiSayisi",
                table: "Randevu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TalepEt",
                table: "Randevu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KisiSayisi",
                table: "Randevu");

            migrationBuilder.DropColumn(
                name: "TalepEt",
                table: "Randevu");

            migrationBuilder.RenameColumn(
                name: "TalepNedeni",
                table: "Randevu",
                newName: "Durum");
        }
    }
}
