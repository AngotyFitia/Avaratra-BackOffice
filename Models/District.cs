using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class District
    {
        [Key]
        [Column("id_district")]
        public int IdDistrict { get; set; }

        [ForeignKey("id_region")]
        public Region region {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal longitude { get; set; }

        [Column("geometrie", TypeName = "geography")]
        public Point geometrie { get; set; }

        [Column("total_population_district")]
        public int totalPopulationDistrict { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}