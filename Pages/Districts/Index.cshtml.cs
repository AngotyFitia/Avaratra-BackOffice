using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Avaratra.BackOffice.Data;
using Avaratra.BackOffice.Models;
using Avaratra.BackOffice.Utils;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.IO;
using System.Text;
using System.Globalization;

namespace Avaratra.BackOffice.Pages_Districts
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<District> Districts { get; set; } = default!;
        public List<Region> RegionsValidees { get; set; } = new();
        
        [BindProperty]
        public District District { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchIntitule { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchRegion { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinPopulation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxPopulation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Etat { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
            const int pageSize = 2;
            var query = _context.District
                                .Include(d => d.Region)   // jointure
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchIntitule))
                query = query.Where(r => r.intitule.Contains(SearchIntitule));
            if (!string.IsNullOrWhiteSpace(SearchRegion))
                query = query.Where(r => r.Region.intitule.Contains(SearchRegion));
            if (MinPopulation.HasValue)
                query = query.Where(r => r.totalPopulationDistrict >= MinPopulation.Value);
            if (MaxPopulation.HasValue)
                query = query.Where(r => r.totalPopulationDistrict <= MaxPopulation.Value);
            if (Etat.HasValue)
                query = query.Where(r => r.etat == Etat.Value);
            query = query.OrderBy(r => r.intitule);
            Districts = await PaginatedList<District>.CreateAsync(query, pageIndex ?? 1, pageSize);

            // Récupération des régions validées
            RegionsValidees = await _context.Region
                                            .Where(r => r.etat == 5)
                                            .OrderBy(r => r.intitule)
                                            .ToListAsync();
            }
            var ditrict = await _context.District.FirstOrDefaultAsync(m => m.idDistrict == id);

            District= ditrict;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Console.WriteLine(District.IdRegion);
            // District.latitude = Convert.ToDecimal(Request.Form["District.latitude"], System.Globalization.CultureInfo.InvariantCulture);
            // District.longitude = Convert.ToDecimal(Request.Form["District.longitude"], System.Globalization.CultureInfo.InvariantCulture);

            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            // District.geometrie = geometryFactory.CreatePoint(new Coordinate((double)District.longitude, (double)District.latitude));
            District.geometrie = geometryFactory.CreatePoint(new Coordinate(0, 0));            
            District.etat = 0;
            _context.District.Add(District);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var districtDb = await _context.District.FindAsync(District.idDistrict);
            if (districtDb == null) return NotFound();
            districtDb.intitule = District.intitule;
            districtDb.IdRegion = District.IdRegion;
            districtDb.totalPopulationDistrict=District.totalPopulationDistrict;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var district = await _context.District.FindAsync(id);
            if (district != null)
            {
                District = district;
                _context.District.Remove(District);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostValidateAsync(int? id)
        {
            var districtDb = await _context.District
                .Include(d => d.Region) 
                .FirstOrDefaultAsync(d => d.idDistrict == id);

            if (districtDb == null) return NotFound();

            districtDb.etat = 5;
            if (districtDb.Region != null)
            {
                districtDb.Region.totalPopulationRegion += districtDb.totalPopulationDistrict;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostConfirmAsync(int? id)
        {
            var districtDb = await _context.District
                .Include(d => d.Region)
                .Include(d => d.Communes) // inclure les communes liées
                .FirstOrDefaultAsync(d => d.idDistrict == id);

            if (districtDb == null) return NotFound();

            // 1. Changer l'état
            districtDb.etat = 10;

            // 3. Construire la géométrie du district à partir des communes
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            // Récupérer les points des communes
            var communePoints = districtDb.Communes
                .Select(c => geometryFactory.CreatePoint(new Coordinate((double)c.longitude, (double)c.latitude)))
                .ToArray();

            if (communePoints.Length > 0)
            {
                // Exemple : créer un MultiPoint puis un polygone englobant
                var multiPoint = geometryFactory.CreateMultiPoint(communePoints);
                districtDb.geometrie = multiPoint.ConvexHull(); // polygone englobant toutes les communes
            }
            else
            {
                // Si pas de communes, garder un point neutre
                districtDb.geometrie = geometryFactory.CreatePoint(new Coordinate(0, 0));
            }

            _context.Update(districtDb);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostImportCsvAsync(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Aucun fichier sélectionné.");
                return Page();
            }
            using var reader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8);

            var header = await reader.ReadLineAsync();
            Console.WriteLine($"Header ignoré: {header}");

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');
                if (values.Length < 6) continue;

                var districtName = values[0].Trim();
                var regionName   = values[1].Trim();
                var longitude    = decimal.Parse(values[2].Trim(), CultureInfo.InvariantCulture);
                var latitude     = decimal.Parse(values[3].Trim(), CultureInfo.InvariantCulture);
                var population   = int.Parse(values[4].Trim(), CultureInfo.InvariantCulture);
                var etat         = int.Parse(values[5].Trim(), CultureInfo.InvariantCulture);

                // Chercher la région existante
                var region = await _context.Region.FirstOrDefaultAsync(r => r.intitule == regionName);
                if (region == null)
                {
                    // Si la région n’existe pas, on peut ignorer ou lever une erreur
                    Console.WriteLine($"Région '{regionName}' introuvable, district '{districtName}' ignoré.");
                    continue;
                }

                // Vérifier si le district existe déjà
                bool existsDistrict = await _context.District.AnyAsync(d => d.intitule == districtName && d.IdRegion == region.idRegion);
                if (!existsDistrict)
                {
                    var district = new District
                    {
                        intitule = districtName,
                        IdRegion = region.idRegion,
                        // latitude = latitude,
                        // longitude = longitude,
                        totalPopulationDistrict = population,
                        etat = etat,
                        geometrie = geometryFactory.CreatePoint(new Coordinate(0, 0))
                    };

                    _context.District.Add(district);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Import des districts terminé avec succès.";
            return RedirectToPage();
        }
        
    }
}
