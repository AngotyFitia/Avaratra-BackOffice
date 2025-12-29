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

        [NotMapped]
        public string EtatText
        {
            get
            {
                return etat switch
                {
                    0 => "En attente",
                    5 => "Validée",
                };
            }
        }

        [NotMapped]
        public string EtatCssClass
        {
            get
            {
                return etat switch
                {
                    0 => "text-warning", 
                    5 => "text-success",  
                };
            }
        }

        [NotMapped]
        public string DisplayCrudButtons
        {
            get
            {
                // Montrer les 3 premiers boutons si etat == 0
                return etat == 0 ? "" : "display:none;";
            }
        }

        [NotMapped]
        public string DisplaySpecialValidation // (etat == 5)
        {
            get
            {
                return etat == 5 ? "" : "display:none;";
            }
        }

    }
}