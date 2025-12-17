using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Unite
    {
        [Key]
        [Column("id_unite")]
        public int idUnite { get; set; }

        [Column("symbole", TypeName = "varchar(20)")]
        public string symbole { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

        public ICollection<Mesure> mesures { get; set; } = new List<Mesure>();


    }
}