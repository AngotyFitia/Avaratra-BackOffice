using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Avaratra.BackOffice.Models
{
    public class Infrastructure
    {
        [Key]
        [Column("id_infrastructure")]
        public int idInfrastructure { get; set; }

        [Column("id_categorie")] 
        public int IdCategorie { get; set; }

        [ValidateNever]
        [ForeignKey("IdCategorie")]
        public Categorie Categorie {get; set;}

        [Column("intitule", TypeName = "varchar(255)")]
        public string intitule { get; set; } = string.Empty;

        [Column("etat")]
        public int etat { get; set; }

    }
}