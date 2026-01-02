using Avaratra.BackOffice.Models;
using System.IO;
using System.Globalization;

namespace Avaratra.BackOffice.Services
{
    public static class RegionService
    {
        public static (Region? entity, string? error) Map(string[] values, int lineNumber)
        {
            if (values.Length < 3)
                return (null, $"Ligne {lineNumber}: nombre de colonnes insuffisant.");

            try
            {
                var intitule = values[0].Trim();
                if (!int.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var population))
                    return (null, $"Ligne {lineNumber}, Colonne 4: population invalide.");

                if (!int.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var etat))
                    return (null, $"Ligne {lineNumber}, Colonne 5: état invalide.");

                var region = new Region
                {
                    intitule = intitule,
                    totalPopulationRegion = 0,
                    etat = etat,
                };

                return (region, null);
            }
            catch (Exception ex)
            {
                return (null, $"Ligne {lineNumber}: erreur inattendue ({ex.Message}).");
            }
        }
    }
}
