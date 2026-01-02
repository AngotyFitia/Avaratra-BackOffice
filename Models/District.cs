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

        // [Display(Name = "Région")] 
        [Required(ErrorMessage = "La région est obligatoire.")]
        [Column("id_region")] 
        public int IdRegion { get; set; }
        
        [Required(ErrorMessage = "La région est obligatoire.")]
        [ValidateNever]
        [ForeignKey("IdRegion")]
        public Region Region {get; set;}

        [Required(ErrorMessage = "Le nom du district est obligatoire.")]
        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [ValidateNever]
        [Column("geometrie", TypeName = "geography")]
        public Geometry geometrie { get; set; }

        [Required(ErrorMessage = "Le nombre de population est obligatoire.")]
        [Range(0, int.MaxValue, ErrorMessage = "La population doit être un nombre positif.")]
        [Column("total_population_district")]
        public int? totalPopulationDistrict { get; set; }

        [Column("etat")]
        public int etat { get; set; }

        [ValidateNever]
        public ICollection<Commune> Communes { get; set; } = new List<Commune>();

        [NotMapped]
        [ValidateNever]
        public string TagRegion { get; set; } = string.Empty;


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
        public string DisplayCrudButtons   // edit / delete / validate classique
        {
            get
            {
                return etat == 0 ? "" : "display:none;";
            }
        }

        [NotMapped]
        public string DisplaySpecialValidation // bouton validation (etat == 5)
        {
            get
            {
                return etat == 5 ? "" : "display:none;";
            }
        }

        [NotMapped]
        public string DisplayFinalValidation // bouton visualisation (etat == 10)
        {
            get
            {
                return etat == 10 ? "" : "display:none;";
            }
        }

    }
}