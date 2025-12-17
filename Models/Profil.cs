using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Profil
    {
        [Key]
        [Column("id_profil")]
        public int idProfil { get; set; }

        [Column("intitule", TypeName = "varchar(50)")]
        public string intitule { get; set; } = string.Empty;

    }
}