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

namespace Avaratra.BackOffice.Pages_Communes
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public PaginatedList<Commune> Communes { get; set; } = default!;
        public List<District> DistrictsValides { get; set; } = new();

        [BindProperty]
        public Commune Commune { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchIntitule { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchRegion { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchDistrict { get; set; }

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
            var query = _context.Commune.Include(c => c.District).ThenInclude(d => d.Region).AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchIntitule))
            query = query.Where(c => c.intitule.Contains(SearchIntitule));
            if (!string.IsNullOrWhiteSpace(SearchRegion))
                query = query.Where(c => c.District.Region.intitule.Contains(SearchRegion));

            if (!string.IsNullOrWhiteSpace(SearchDistrict))
                query = query.Where(c => c.District.intitule.Contains(SearchDistrict));

            if (MinPopulation.HasValue)
                query = query.Where(c => c.nombrePopulation >= MinPopulation.Value);

            if (MaxPopulation.HasValue)
                query = query.Where(c => c.nombrePopulation <= MaxPopulation.Value);

            if (Etat.HasValue)
                query = query.Where(c => c.etat == Etat.Value);

            query = query.OrderBy(r => r.intitule);
            Communes = await PaginatedList<Commune>.CreateAsync(query, pageIndex ?? 1, pageSize);

            // Récupération des régions validées
            DistrictsValides = await _context.District
                                            .Where(r => r.etat == 5)
                                            .OrderBy(r => r.intitule)
                                            .ToListAsync();
            }
            var commune = await _context.Commune.FirstOrDefaultAsync(m => m.idCommune == id);

            Commune= commune;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Console.WriteLine(Commune.IdDistrict);
            Commune.latitude = Convert.ToDecimal(Request.Form["Commune.latitude"], System.Globalization.CultureInfo.InvariantCulture);
            Commune.longitude = Convert.ToDecimal(Request.Form["Commune.longitude"], System.Globalization.CultureInfo.InvariantCulture);

            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Commune.geometrie = geometryFactory.CreatePoint(new Coordinate((double)Commune.longitude, (double)Commune.latitude));
            Commune.etat = 0;
            _context.Commune.Add(Commune);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var communeDb = await _context.Commune.FindAsync(Commune.idCommune);
            if (communeDb == null) return NotFound();
            communeDb.intitule = Commune.intitule;
            communeDb.IdDistrict = Commune.IdDistrict;
            communeDb.nombrePopulation=Commune.nombrePopulation;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var commune = await _context.Commune.FindAsync(id);
            if (commune != null)
            {
                Commune = commune;
                _context.Commune.Remove(Commune);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostValidateAsync(int? id)
        {
            var communeDb = await _context.Commune
                .Include(c => c.District)
                .FirstOrDefaultAsync(c => c.idCommune == id);

            if (communeDb.District != null)
            {
                Console.WriteLine($"Avant: {communeDb.District.totalPopulationDistrict}");
                communeDb.District.totalPopulationDistrict += communeDb.nombrePopulation;
                Console.WriteLine($"Après: {communeDb.District.totalPopulationDistrict}");
            }

            communeDb.etat = 5;
            if (communeDb.District != null)
            {
                communeDb.District.totalPopulationDistrict += communeDb.nombrePopulation;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

    }
}
