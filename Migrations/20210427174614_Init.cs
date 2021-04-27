using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZespolServer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KierownicyZespolow",
                columns: table => new
                {
                    PESEL = table.Column<string>(type: "text", nullable: false),
                    Doswiadczenie = table.Column<int>(type: "integer", nullable: false),
                    Imie = table.Column<string>(type: "text", nullable: true),
                    Nazwisko = table.Column<string>(type: "text", nullable: true),
                    DataUrodzenia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Plec = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KierownicyZespolow", x => x.PESEL);
                });

            migrationBuilder.CreateTable(
                name: "Zespoly",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nazwa = table.Column<string>(type: "text", nullable: true),
                    KierownikPESEL = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zespoly", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zespoly_KierownicyZespolow_KierownikPESEL",
                        column: x => x.KierownikPESEL,
                        principalTable: "KierownicyZespolow",
                        principalColumn: "PESEL",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CzlonkowieZespolow",
                columns: table => new
                {
                    PESEL = table.Column<string>(type: "text", nullable: false),
                    DataZapisu = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Funkcja = table.Column<string>(type: "text", nullable: true),
                    ZespolId = table.Column<string>(type: "text", nullable: true),
                    Imie = table.Column<string>(type: "text", nullable: true),
                    Nazwisko = table.Column<string>(type: "text", nullable: true),
                    DataUrodzenia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Plec = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CzlonkowieZespolow", x => x.PESEL);
                    table.ForeignKey(
                        name: "FK_CzlonkowieZespolow_Zespoly_ZespolId",
                        column: x => x.ZespolId,
                        principalTable: "Zespoly",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZespolClusters",
                columns: table => new
                {
                    Nazwa = table.Column<string>(type: "text", nullable: false),
                    ZespolId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZespolClusters", x => x.Nazwa);
                    table.ForeignKey(
                        name: "FK_ZespolClusters_Zespoly_ZespolId",
                        column: x => x.ZespolId,
                        principalTable: "Zespoly",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZespolArchive",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ZespolClusterNazwa = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZespolArchive", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZespolArchive_ZespolClusters_ZespolClusterNazwa",
                        column: x => x.ZespolClusterNazwa,
                        principalTable: "ZespolClusters",
                        principalColumn: "Nazwa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CzlonkowieZespolow_ZespolId",
                table: "CzlonkowieZespolow",
                column: "ZespolId");

            migrationBuilder.CreateIndex(
                name: "IX_ZespolArchive_ZespolClusterNazwa",
                table: "ZespolArchive",
                column: "ZespolClusterNazwa");

            migrationBuilder.CreateIndex(
                name: "IX_ZespolClusters_ZespolId",
                table: "ZespolClusters",
                column: "ZespolId");

            migrationBuilder.CreateIndex(
                name: "IX_Zespoly_KierownikPESEL",
                table: "Zespoly",
                column: "KierownikPESEL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CzlonkowieZespolow");

            migrationBuilder.DropTable(
                name: "ZespolArchive");

            migrationBuilder.DropTable(
                name: "ZespolClusters");

            migrationBuilder.DropTable(
                name: "Zespoly");

            migrationBuilder.DropTable(
                name: "KierownicyZespolow");
        }
    }
}
