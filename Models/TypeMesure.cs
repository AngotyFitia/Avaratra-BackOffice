using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class TypeMesure
    {
        [Key]
        [Column("id_type_mesure")]
        public int idTypeMesure { get; set; }

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("description")]
        [Required, MaxLength(255)]
        public string description { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

    }
}