using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Avaratra.BackOffice.Models
{
    public class District
    {
        [Key]
        [Column("id_district")]
        public int idDistrict { get; set; }

        [Column("id_region")] 
        public int IdRegion { get; set; }
        
        [ValidateNever]
        [ForeignKey("IdRegion")]
        public Region Region {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal longitude { get; set; }

        [ValidateNever]
        [Column("geometrie", TypeName = "geography")]
        public Point geometrie { get; set; }

        [Column("total_population_district")]
        public int totalPopulationDistrict { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}