using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Signalement
    {
        [Key]
        [Column("id_signalement")]
        public int idSignalement { get; set; }

        [ForeignKey("id_commune")]
        public Commune commune {get; set;}

        [Column("id_type_signalement")]
        public int idTypeSignalement { get; set; }
        public TypeSignalement typeSignalement { get; set; }

        [Column("id_utilisateur")]
        public int idUtilisateur { get; set; }
        public Utilisateur utilisateur { get; set; }

        [Column("description")]
        public string description { get; set; } = string.Empty;

        [Column("photo")]
        public byte photo { get; set; }

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal longitude { get; set; }

        [Column("date_signalement")]
        public DateTime dateSignalement { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}