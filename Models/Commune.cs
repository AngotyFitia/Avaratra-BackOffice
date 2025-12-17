using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Commune
    {
        [Key]
        [Column("id_commune")]
        public int idCommune { get; set; }

        [ForeignKey("id_district")]
        public District district {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal longitude { get; set; }

        [Column("geometrie", TypeName = "geography")]
        public Point geometrie { get; set; }

        [Column("nombre_population")]
        public int nombrePopulation { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}