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


namespace Avaratra.BackOffice.Pages_Districts
{
    public class IndexModel : PageModel
    {
        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;
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
            District.latitude = Convert.ToDecimal(Request.Form["District.latitude"], System.Globalization.CultureInfo.InvariantCulture);
            District.longitude = Convert.ToDecimal(Request.Form["District.longitude"], System.Globalization.CultureInfo.InvariantCulture);

            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            District.geometrie = geometryFactory.CreatePoint(new Coordinate((double)District.longitude, (double)District.latitude));
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
            var districtDb = await _context.District.FindAsync(id);
            if (districtDb == null) return NotFound();

            districtDb.etat = 5;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        
    }
}
