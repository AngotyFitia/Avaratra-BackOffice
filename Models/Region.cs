using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Region
    {
        [Key]
        [Column("id_region")]
        public int idRegion { get; set; }

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal longitude { get; set; }

        [Column("geometrie", TypeName = "geography")]
        public Point geometrie { get; set; }

        [Column("total_population_region")]
        public int totalPopulationRegion { get; set; }

        [Column("etat")]
        public int etat { get; set; }
    }
}