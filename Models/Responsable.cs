using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Responsable
    {
        [Key]
        [Column("id_responsable")]
        public int idResponsable { get; set; }

        [Column("id_utilisateur")]
        public int idUtilisateur { get; set; }
        public Utilisateur utilisateur { get; set; }

        [ForeignKey("id_commune")]
        public Commune commune {get; set;}

        [Column("date_debut")]
        public DateTime dateDebut { get; set; }

        [Column("date_fin")]
        public DateTime dateFin { get; set; }

        [Column("etat")]
        public int etat { get; set; }

    }
}