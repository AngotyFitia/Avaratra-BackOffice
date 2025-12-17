using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class TypeAlerte
    {
        [Key]
        [Column("id_type_alerte")]
        public int idTypeAlerte { get; set; }

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

    }
}