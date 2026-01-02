using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Avaratra.BackOffice.Models
{
    public class Region
    {
        [Key]
        [Column("id_region")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idRegion { get; set; }

        [Required(ErrorMessage = "Le nom de la région est obligatoire.")]
        [Column("intitule", TypeName = "varchar(255)")]
        public string? intitule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nombre de population est obligatoire.")]
        [Range(0, int.MaxValue, ErrorMessage = "La population doit être un nombre positif.")]
        [Column("total_population_region")]
        public int? totalPopulationRegion { get; set; }

        [Column("etat")]
        public int etat { get; set; }

        public ICollection<District> Districts { get; set; } = new List<District>();


        [NotMapped]
        public string EtatText
        {
            get
            {
                return etat switch
                {
                    0 => "En attente",
                    5 => "Validée",
                    10 => "Finalisé"
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
                    10 => "text-muted" 
                };
            }
        }

        [NotMapped]
        public string DisplayCrudButtons  
        {
            get
            {
                return etat == 0 ? "" : "display:none;";
            }
        }

        [NotMapped]
        public string DisplayView 
        {
            get
            {
                return etat == 5 ? "" : "display:none;";
            }
        }
    }
}