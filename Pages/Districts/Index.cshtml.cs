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
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;
        public PaginatedList<District> Districts { get; set; } = default!;
        public List<Region> RegionsValidees { get; set; } = new();

        [BindProperty]
        public District District { get; set; } = default!;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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

        public async Task<IActionResult> OnGetAsync(int? pageIndex)
        {
            const int pageSize = 10;
            // Récupération des districts avec leur région
            var queryDistricts = _context.District
                                        .Include(d => d.Region)
                                        .OrderBy(d => d.intitule);
            Districts = await PaginatedList<District>.CreateAsync(queryDistricts, pageIndex ?? 1, pageSize);

            // Récupération des régions validées
            RegionsValidees = await _context.Region
                                            .Where(r => r.etat == 5)
                                            .OrderBy(r => r.intitule)
                                            .ToListAsync();
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
    }
}
