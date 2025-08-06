using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AracTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciTablosuSadelesti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_Araclar_AracID",
                table: "Randevular");

            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_Kullanicilar_KullaniciID",
                table: "Randevular");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Randevular",
                table: "Randevular");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Araclar",
                table: "Araclar");

            migrationBuilder.DropColumn(
                name: "Ad",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Kullanicilar");

            migrationBuilder.RenameTable(
                name: "Randevular",
                newName: "Randevu");

            migrationBuilder.RenameTable(
                name: "Araclar",
                newName: "Arac");

            migrationBuilder.RenameColumn(
                name: "Telefon",
                table: "Kullanicilar",
                newName: "KullaniciAdi");

            migrationBuilder.RenameColumn(
                name: "Soyad",
                table: "Kullanicilar",
                newName: "Departman");

            migrationBuilder.RenameIndex(
                name: "IX_Randevular_KullaniciID",
                table: "Randevu",
                newName: "IX_Randevu_KullaniciID");

            migrationBuilder.RenameIndex(
                name: "IX_Randevular_AracID",
                table: "Randevu",
                newName: "IX_Randevu_AracID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Randevu",
                table: "Randevu",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Arac",
                table: "Arac",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevu_Arac_AracID",
                table: "Randevu",
                column: "AracID",
                principalTable: "Arac",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Randevu_Kullanicilar_KullaniciID",
                table: "Randevu",
                column: "KullaniciID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevu_Arac_AracID",
                table: "Randevu");

            migrationBuilder.DropForeignKey(
                name: "FK_Randevu_Kullanicilar_KullaniciID",
                table: "Randevu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Randevu",
                table: "Randevu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Arac",
                table: "Arac");

            migrationBuilder.RenameTable(
                name: "Randevu",
                newName: "Randevular");

            migrationBuilder.RenameTable(
                name: "Arac",
                newName: "Araclar");

            migrationBuilder.RenameColumn(
                name: "KullaniciAdi",
                table: "Kullanicilar",
                newName: "Telefon");

            migrationBuilder.RenameColumn(
                name: "Departman",
                table: "Kullanicilar",
                newName: "Soyad");

            migrationBuilder.RenameIndex(
                name: "IX_Randevu_KullaniciID",
                table: "Randevular",
                newName: "IX_Randevular_KullaniciID");

            migrationBuilder.RenameIndex(
                name: "IX_Randevu_AracID",
                table: "Randevular",
                newName: "IX_Randevular_AracID");

            migrationBuilder.AddColumn<string>(
                name: "Ad",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Randevular",
                table: "Randevular",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Araclar",
                table: "Araclar",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_Araclar_AracID",
                table: "Randevular",
                column: "AracID",
                principalTable: "Araclar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_Kullanicilar_KullaniciID",
                table: "Randevular",
                column: "KullaniciID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
