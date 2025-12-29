using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Avaratra.BackOffice.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorie",
                columns: table => new
                {
                    id_categorie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorie", x => x.id_categorie);
                });

            migrationBuilder.CreateTable(
                name: "Profil",
                columns: table => new
                {
                    id_profil = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    intitule = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profil", x => x.id_profil);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    id_region = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    geometrie = table.Column<Point>(type: "geography", nullable: false),
                    total_population_region = table.Column<int>(type: "int", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.id_region);
                });

            migrationBuilder.CreateTable(
                name: "TypeAlerte",
                columns: table => new
                {
                    id_type_alerte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeAlerte", x => x.id_type_alerte);
                });

            migrationBuilder.CreateTable(
                name: "TypeSignalement",
                columns: table => new
                {
                    id_type_signalement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeSignalement", x => x.id_type_signalement);
                });

            migrationBuilder.CreateTable(
                name: "Unite",
                columns: table => new
                {
                    id_unite = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    symbole = table.Column<string>(type: "varchar(20)", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unite", x => x.id_unite);
                });

            migrationBuilder.CreateTable(
                name: "Infrastructure",
                columns: table => new
                {
                    id_infrastructure = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_categorie = table.Column<int>(type: "int", nullable: false),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infrastructure", x => x.id_infrastructure);
                    table.ForeignKey(
                        name: "FK_Infrastructure_Categorie_id_categorie",
                        column: x => x.id_categorie,
                        principalTable: "Categorie",
                        principalColumn: "id_categorie",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    id_district = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_region = table.Column<int>(type: "int", nullable: false),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    geometrie = table.Column<Point>(type: "geography", nullable: false),
                    total_population_district = table.Column<int>(type: "int", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_District", x => x.id_district);
                    table.ForeignKey(
                        name: "FK_District_Region_id_region",
                        column: x => x.id_region,
                        principalTable: "Region",
                        principalColumn: "id_region",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypeMesure",
                columns: table => new
                {
                    id_type_mesure = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_unite = table.Column<int>(type: "int", nullable: false),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeMesure", x => x.id_type_mesure);
                    table.ForeignKey(
                        name: "FK_TypeMesure_Unite_id_unite",
                        column: x => x.id_unite,
                        principalTable: "Unite",
                        principalColumn: "id_unite",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commune",
                columns: table => new
                {
                    id_commune = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_district = table.Column<int>(type: "int", nullable: false),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    geometrie = table.Column<Point>(type: "geography", nullable: false),
                    nombre_population = table.Column<int>(type: "int", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commune", x => x.id_commune);
                    table.ForeignKey(
                        name: "FK_Commune_District_id_district",
                        column: x => x.id_district,
                        principalTable: "District",
                        principalColumn: "id_district",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerte",
                columns: table => new
                {
                    id_alerte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_commune = table.Column<int>(type: "int", nullable: false),
                    id_type_alerte = table.Column<int>(type: "int", nullable: false),
                    typeAlerteidTypeAlerte = table.Column<int>(type: "int", nullable: false),
                    niveau = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_debut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_fin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerte", x => x.id_alerte);
                    table.ForeignKey(
                        name: "FK_Alerte_Commune_id_commune",
                        column: x => x.id_commune,
                        principalTable: "Commune",
                        principalColumn: "id_commune",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alerte_TypeAlerte_typeAlerteidTypeAlerte",
                        column: x => x.typeAlerteidTypeAlerte,
                        principalTable: "TypeAlerte",
                        principalColumn: "id_type_alerte",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Capteur",
                columns: table => new
                {
                    id_capteur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_infrastructure = table.Column<int>(type: "int", nullable: false),
                    id_commune = table.Column<int>(type: "int", nullable: false),
                    intitule = table.Column<string>(type: "varchar(255)", nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    date_debut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capteur", x => x.id_capteur);
                    table.ForeignKey(
                        name: "FK_Capteur_Commune_id_commune",
                        column: x => x.id_commune,
                        principalTable: "Commune",
                        principalColumn: "id_commune",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Capteur_Infrastructure_id_infrastructure",
                        column: x => x.id_infrastructure,
                        principalTable: "Infrastructure",
                        principalColumn: "id_infrastructure",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    id_utilisateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_profil = table.Column<int>(type: "int", nullable: false),
                    id_commune = table.Column<int>(type: "int", nullable: false),
                    nom = table.Column<string>(type: "varchar(255)", nullable: false),
                    prenoms = table.Column<string>(type: "varchar(255)", nullable: false),
                    email = table.Column<string>(type: "varchar(255)", nullable: false),
                    mot_de_passe = table.Column<string>(type: "varchar(255)", nullable: false),
                    date_naissance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_creation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.id_utilisateur);
                    table.ForeignKey(
                        name: "FK_Utilisateur_Commune_id_commune",
                        column: x => x.id_commune,
                        principalTable: "Commune",
                        principalColumn: "id_commune",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Utilisateur_Profil_id_profil",
                        column: x => x.id_profil,
                        principalTable: "Profil",
                        principalColumn: "id_profil",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mesure",
                columns: table => new
                {
                    id_mesure = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_capteur = table.Column<int>(type: "int", nullable: false),
                    id_type_mesure = table.Column<int>(type: "int", nullable: false),
                    id_unite = table.Column<int>(type: "int", nullable: false),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    valeur = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    date_prise = table.Column<DateTime>(type: "datetime2", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesure", x => x.id_mesure);
                    table.ForeignKey(
                        name: "FK_Mesure_Capteur_id_capteur",
                        column: x => x.id_capteur,
                        principalTable: "Capteur",
                        principalColumn: "id_capteur",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mesure_TypeMesure_id_type_mesure",
                        column: x => x.id_type_mesure,
                        principalTable: "TypeMesure",
                        principalColumn: "id_type_mesure",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mesure_Unite_id_unite",
                        column: x => x.id_unite,
                        principalTable: "Unite",
                        principalColumn: "id_unite",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mesure_Utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Responsable",
                columns: table => new
                {
                    id_responsable = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    id_commune = table.Column<int>(type: "int", nullable: false),
                    date_debut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsable", x => x.id_responsable);
                    table.ForeignKey(
                        name: "FK_Responsable_Commune_id_commune",
                        column: x => x.id_commune,
                        principalTable: "Commune",
                        principalColumn: "id_commune",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Responsable_Utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Signalement",
                columns: table => new
                {
                    id_signalement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_commune = table.Column<int>(type: "int", nullable: false),
                    id_type_signalement = table.Column<int>(type: "int", nullable: false),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    photo = table.Column<byte>(type: "tinyint", nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    date_signalement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signalement", x => x.id_signalement);
                    table.ForeignKey(
                        name: "FK_Signalement_Commune_id_commune",
                        column: x => x.id_commune,
                        principalTable: "Commune",
                        principalColumn: "id_commune",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Signalement_TypeSignalement_id_type_signalement",
                        column: x => x.id_type_signalement,
                        principalTable: "TypeSignalement",
                        principalColumn: "id_type_signalement",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Signalement_Utilisateur_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "Utilisateur",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerte_id_commune",
                table: "Alerte",
                column: "id_commune");

            migrationBuilder.CreateIndex(
                name: "IX_Alerte_typeAlerteidTypeAlerte",
                table: "Alerte",
                column: "typeAlerteidTypeAlerte");

            migrationBuilder.CreateIndex(
                name: "IX_Capteur_id_commune",
                table: "Capteur",
                column: "id_commune");

            migrationBuilder.CreateIndex(
                name: "IX_Capteur_id_infrastructure",
                table: "Capteur",
                column: "id_infrastructure");

            migrationBuilder.CreateIndex(
                name: "IX_Commune_id_district",
                table: "Commune",
                column: "id_district");

            migrationBuilder.CreateIndex(
                name: "IX_District_id_region",
                table: "District",
                column: "id_region");

            migrationBuilder.CreateIndex(
                name: "IX_Infrastructure_id_categorie",
                table: "Infrastructure",
                column: "id_categorie");

            migrationBuilder.CreateIndex(
                name: "IX_Mesure_id_capteur",
                table: "Mesure",
                column: "id_capteur");

            migrationBuilder.CreateIndex(
                name: "IX_Mesure_id_type_mesure",
                table: "Mesure",
                column: "id_type_mesure");

            migrationBuilder.CreateIndex(
                name: "IX_Mesure_id_unite",
                table: "Mesure",
                column: "id_unite");

            migrationBuilder.CreateIndex(
                name: "IX_Mesure_id_utilisateur",
                table: "Mesure",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Responsable_id_commune",
                table: "Responsable",
                column: "id_commune");

            migrationBuilder.CreateIndex(
                name: "IX_Responsable_id_utilisateur",
                table: "Responsable",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Signalement_id_commune",
                table: "Signalement",
                column: "id_commune");

            migrationBuilder.CreateIndex(
                name: "IX_Signalement_id_type_signalement",
                table: "Signalement",
                column: "id_type_signalement");

            migrationBuilder.CreateIndex(
                name: "IX_Signalement_id_utilisateur",
                table: "Signalement",
                column: "id_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_TypeMesure_id_unite",
                table: "TypeMesure",
                column: "id_unite");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateur_id_commune",
                table: "Utilisateur",
                column: "id_commune");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateur_id_profil",
                table: "Utilisateur",
                column: "id_profil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerte");

            migrationBuilder.DropTable(
                name: "Mesure");

            migrationBuilder.DropTable(
                name: "Responsable");

            migrationBuilder.DropTable(
                name: "Signalement");

            migrationBuilder.DropTable(
                name: "TypeAlerte");

            migrationBuilder.DropTable(
                name: "Capteur");

            migrationBuilder.DropTable(
                name: "TypeMesure");

            migrationBuilder.DropTable(
                name: "TypeSignalement");

            migrationBuilder.DropTable(
                name: "Utilisateur");

            migrationBuilder.DropTable(
                name: "Infrastructure");

            migrationBuilder.DropTable(
                name: "Unite");

            migrationBuilder.DropTable(
                name: "Commune");

            migrationBuilder.DropTable(
                name: "Profil");

            migrationBuilder.DropTable(
                name: "Categorie");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
