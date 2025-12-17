using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class TypeSignalement
    {
        [Key]
        [Column("id_type_signalement")]
        public int idTypeSignalement { get; set; }

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

        public ICollection<Signalement> signalements { get; set; } = new List<Signalement>();

    }
}