using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace B2BUygulamasi.Migrations
{
    /// <inheritdoc />
    public partial class RollerVeIliskilerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmaAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.KullaniciID);
                });

            migrationBuilder.CreateTable(
                name: "Roller",
                columns: table => new
                {
                    RolID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roller", x => x.RolID);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciRolleri",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    RolID = table.Column<int>(type: "int", nullable: false),
                    KullaniciRolID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciRolleri", x => new { x.KullaniciID, x.RolID });
                    table.ForeignKey(
                        name: "FK_KullaniciRolleri_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciRolleri_Roller_RolID",
                        column: x => x.RolID,
                        principalTable: "Roller",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roller",
                columns: new[] { "RolID", "RolAdi" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Standart" },
                    { 3, "FirmaAdmin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRolleri_RolID",
                table: "KullaniciRolleri",
                column: "RolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciRolleri");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Roller");
        }
    }
}
