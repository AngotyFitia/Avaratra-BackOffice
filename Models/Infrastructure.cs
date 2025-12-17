using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
namespace Avaratra.BackOffice.Models
{
    public class Infrastructure
    {
        [Key]
        [Column("id_infrastructure")]
        public int idInfrastructure { get; set; }

        [ForeignKey("id_categorie")]
        public Categorie categorie {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

    }
}