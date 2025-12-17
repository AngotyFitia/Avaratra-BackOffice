using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Capteur
    {
        [Key]
        [Column("id_capteur")]
        public int idCapteur { get; set; }

        [ForeignKey("id_infrastructure")]
        public Infrastructure infrastructure {get; set;}

        [ForeignKey("id_commune")]
        public Commune commune {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("latitude", TypeName = "decimal(9,6)")]
        public decimal latitude { get; set; }

        [Column("longitude", TypeName = "decimal(9,6)")]
        public decimal longitude { get; set; }

        [Column("date_debut")]
        public DateTime dateDebut { get; set; }

        [Column("date_fin")]
        public DateTime dateFin { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}