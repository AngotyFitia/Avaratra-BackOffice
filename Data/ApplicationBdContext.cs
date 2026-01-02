using Microsoft.EntityFrameworkCore;
using Avaratra.BackOffice.Models;

namespace Avaratra.BackOffice.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Region> Region { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Commune> Commune { get; set; }
        public DbSet<Profil> Profil { get; set; }
        public DbSet<Utilisateur> Utilisateur { get; set; }
        public DbSet<Responsable> Responsable { get; set; }
        public DbSet<Categorie> Categorie { get; set; }
        public DbSet<Infrastructure> Infrastructure { get; set; }
        public DbSet<Capteur> Capteur { get; set; }
        public DbSet<Unite> Unite { get; set; }
        public DbSet<TypeMesure> TypeMesure { get; set; }
        public DbSet<Mesure> Mesure { get; set; }
        public DbSet<TypeSignalement> TypeSignalement { get; set; }
        public DbSet<Signalement> Signalement { get; set; }
        public DbSet<TypeAlerte> TypeAlerte { get; set; }
        public DbSet<Alerte> Alerte { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<District>()
                .HasOne(d => d.Region)                // un district a une région
                .WithMany(r => r.Districts)           // une région a plusieurs districts
                .HasForeignKey(d => d.IdRegion)       // clé étrangère
                .HasPrincipalKey(r => r.idRegion);    // clé primaire

            modelBuilder.Entity<Commune>()
                .HasOne(d => d.District)                // une commune a un district
                .WithMany(c => c.Communes)           // une district a plusieurs communnes
                .HasForeignKey(d => d.IdDistrict)       // clé étrangère
                .HasPrincipalKey(c => c.idDistrict); 

            modelBuilder.Entity<Mesure>()
                .HasOne(m => m.Utilisateur)
                .WithMany(u => u.mesures)
                .HasForeignKey(m => m.IdUtilisateur)
                .OnDelete(DeleteBehavior.Restrict); // pas de cascade

            modelBuilder.Entity<Mesure>()
                .HasOne(m => m.Unite)
                .WithMany(u => u.mesures)
                .HasForeignKey(m => m.IdUnite)
                .OnDelete(DeleteBehavior.Restrict); // pas de cascade

            modelBuilder.Entity<Responsable>()
                .HasOne(r => r.utilisateur)
                .WithMany(u => u.responsables)
                .HasForeignKey(r => r.idUtilisateur)
                .OnDelete(DeleteBehavior.Restrict); // évite le cycle de cascade
            
            modelBuilder.Entity<Signalement>()
                        .HasOne(s => s.utilisateur)
                        .WithMany(u => u.signalements)
                        .HasForeignKey(s => s.idUtilisateur)
                        .OnDelete(DeleteBehavior.Restrict); // pas de cascade

            modelBuilder.Entity<Signalement>()
                .HasOne(s => s.typeSignalement)
                .WithMany(t => t.signalements)
                .HasForeignKey(s => s.idTypeSignalement)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
