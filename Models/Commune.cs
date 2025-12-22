using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Avaratra.BackOffice.Models
{
    public class Commune
    {
        [Key]
        [Column("id_commune")]
        public int idCommune { get; set; }
        
        [Column("id_district")] 
        public int IdDistrict { get; set; }

        [ValidateNever]
        [ForeignKey("IdDistrict")]
        public District District {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal longitude { get; set; }

        [ValidateNever]
        [Column("geometrie", TypeName = "geography")]
        public Point geometrie { get; set; }

        [Column("nombre_population")]
        public int nombrePopulation { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}