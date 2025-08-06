using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class MakeAracIDNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevu_Arac_AracID",
                table: "Randevu");

            migrationBuilder.AlterColumn<int>(
                name: "AracID",
                table: "Randevu",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevu_Arac_AracID",
                table: "Randevu",
                column: "AracID",
                principalTable: "Arac",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevu_Arac_AracID",
                table: "Randevu");

            migrationBuilder.AlterColumn<int>(
                name: "AracID",
                table: "Randevu",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Randevu_Arac_AracID",
                table: "Randevu",
                column: "AracID",
                principalTable: "Arac",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
