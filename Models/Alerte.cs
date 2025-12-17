using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Alerte
    {
        [Key]
        [Column("id_alerte")]
        public int idAlerte { get; set; }

        [ForeignKey("id_commune")]
        public Commune commune {get; set;}

        [Column("id_type_alerte")]
        public int idTypeAlerte { get; set; }
        public TypeAlerte typeAlerte { get; set; }

        [Column("niveau", TypeName = "decimal(9,6)")]
        public decimal niveau { get; set; }

        [Column("message")]
        public string message { get; set; } = string.Empty;

        [Column("date_debut")]
        public DateTime dateDebut { get; set; }

        [Column("date_fin")]
        public DateTime? dateFin { get; set; }

        [Column("etat")]
        public int etat { get; set; }   

    }
}