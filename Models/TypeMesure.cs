using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Avaratra.BackOffice.Models
{
    public class TypeMesure
    {
        [Key]
        [Column("id_type_mesure")]
        public int idTypeMesure { get; set; }

        [Column("id_unite")] 
        public int IdUnite { get; set; }

        [ValidateNever]
        [ForeignKey("IdUnite")]
        public Unite Unite {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("description")]
        [Required, MaxLength(255)]
        public string description { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

    }
}