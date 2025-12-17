using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Categorie
    {
        [Key]
        [Column("id_categorie")]
        public int idCategorie { get; set; }

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

    }
}