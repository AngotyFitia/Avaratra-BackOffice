using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Avaratra.BackOffice.Models
{
    public class Utilisateur
    {
        [Key]
        [Column("id_utilisateur")]
        public int idUtilisateur { get; set; }
        
        [Column("id_profil")] 
        public int IdProfil { get; set; }

        [ValidateNever]
        [ForeignKey("IdProfil")]
        public Profil Profil {get; set;}

        [Column("id_commune")] 
        public int IdCommune { get; set; }

        [ValidateNever]
        [ForeignKey("IdCommune")]
        public Commune Commune {get; set;}

        [Column("nom", TypeName = "varchar(255)")]
        public string nom { get; set; } = string.Empty;

        [Column("prenoms", TypeName = "varchar(255)")]
        public string prenoms { get; set; } = string.Empty;

        [Column("email", TypeName = "varchar(255)")]
        public string email { get; set; } = string.Empty;

        [Column("mot_de_passe", TypeName = "varchar(255)")]
        public string motDePasse { get; set; } = string.Empty;

        [Column("date_naissance")]
        public DateTime dateNaissance { get; set; }

        [Column("date_creation")]
        public DateTime dateCreation { get; set; } = DateTime.Now;

        [Column("etat")]
        public int etat { get; set; }

        public ICollection<Mesure> mesures { get; set; } = new List<Mesure>();
        public ICollection<Responsable> responsables { get; set; } = new List<Responsable>();
        public ICollection<Signalement> signalements { get; set; } = new List<Signalement>();

    }
}