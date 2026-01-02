using Avaratra.BackOffice.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace Avaratra.BackOffice.Services.Importing
{
    public static class DistrictService
    {
        public static (District? entity, string? error) Map(string[] values, int lineNumber)
        {
            if (values.Length < 4)
                return (null, $"Ligne {lineNumber}: nombre de colonnes insuffisant.");

            try
            {
                var intitule = values[0].Trim();
                var region   = values[1].Trim();

                if (!int.TryParse(values[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var population))
                    return (null, $"Line {lineNumber}, Column 3: nombre de population invalide.");

                if (!int.TryParse(values[3].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var etat))
                    return (null, $"Line {lineNumber}, Column 4: état invalide.");

                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var district = new District
                {
                    intitule = intitule,
                    totalPopulationDistrict = 0,
                    etat = etat,
                    geometrie = geometryFactory.CreatePoint(new Coordinate(0,0))
                };
                district.TagRegion = region;
                return (district, null);
            }
            catch (Exception ex)
            {
                return (null, $"Line {lineNumber}: unexpected error ({ex.Message}).");
            }
        }
    }
}
