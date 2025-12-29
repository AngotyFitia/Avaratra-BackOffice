using Microsoft.EntityFrameworkCore;
using Avaratra.BackOffice.Models;
namespace Avaratra.BackOffice.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // éviter d’insérer un profil déjà existant mais ajouter les manquants
                var profils = new List<Profil>
                {
                    new Profil { intitule = "Admin" },
                    new Profil { intitule = "Ministère" },
                    new Profil { intitule = "ONG" },
                    new Profil { intitule = "Commune" },
                    new Profil { intitule = "Citoyen" }
                };

                foreach (var profil in profils)
                {
                    if (!context.Profil.Any(p => p.intitule == profil.intitule))
                    {
                        context.Profil.Add(profil);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
