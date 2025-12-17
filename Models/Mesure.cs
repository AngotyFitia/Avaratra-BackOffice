
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Mesure
    {
        [Key]
        [Column("id_mesure")]
        public int idMesure { get; set; }

        [ForeignKey("id_capteur")]
        public Capteur capteur {get; set;}

        [ForeignKey("id_type_mesure")]
        public TypeMesure typeMesure {get; set;}

        [Column("id_unite")]
        public int idUnite { get; set; }
        public Unite unite { get; set; }

        [Column("id_utilisateur")]
        public int idUtilisateur { get; set; }
        public Utilisateur utilisateur { get; set; }

        [Column("valeur", TypeName = "decimal(9,6)")]
        public decimal valeur { get; set; }

        [Column("date_prise")]
        public DateTime datePrise { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}