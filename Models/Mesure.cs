
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Avaratra.BackOffice.Models
{
    public class Mesure
    {
        [Key]
        [Column("id_mesure")]
        public int idMesure { get; set; }

        [Column("id_capteur")] 
        public int IdCapteur { get; set; }

        [ValidateNever]
        [ForeignKey("IdCapteur")]
        public Capteur Capteur {get; set;}

        [Column("id_type_mesure")] 
        public int IdTypeMesure { get; set; }

        [ValidateNever]
        [ForeignKey("IdTypeMesure")]
        public TypeMesure TypeMesure {get; set;}

        [Column("id_unite")] 
        public int IdUnite { get; set; }

        [ValidateNever]
        [ForeignKey("IdUnite")]
        public Unite Unite {get; set;}

        [Column("id_utilisateur")] 
        public int IdUtilisateur { get; set; }

        [ValidateNever]
        [ForeignKey("IdUtilisateur")]
        public Utilisateur Utilisateur {get; set;}

        [Column("valeur", TypeName = "decimal(9,6)")]
        public decimal valeur { get; set; }

        [Column("date_prise")]
        public DateTime datePrise { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}