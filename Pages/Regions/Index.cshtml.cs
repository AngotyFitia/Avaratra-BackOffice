using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Avaratra.BackOffice.Data;
using Avaratra.BackOffice.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Globalization;


namespace Avaratra.BackOffice.Pages_Regions
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;
        public IList<Region> Regions { get;set; } = default!; // tableau de region

        [BindProperty]
        public Region Region { get; set; } = default!; //une seule region

        [BindProperty]
        public List<int> SelectedIds { get; set; } = new();

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        // public async Task OnGetAsync()
        // {
        //     if (_context.Region != null)
        //     {
        //         Regions = await _context.Region.ToListAsync();
        //     }
        // }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Region.latitude = Convert.ToDecimal(Request.Form["Region.latitude"], System.Globalization.CultureInfo.InvariantCulture);
            Region.longitude = Convert.ToDecimal(Request.Form["Region.longitude"], System.Globalization.CultureInfo.InvariantCulture);

            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            Region.geometrie = geometryFactory.CreatePoint(new Coordinate((double)Region.longitude, (double)Region.latitude));
            Region.etat = 0;
            _context.Region.Add(Region);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                Regions = await _context.Region.ToListAsync();
                return Page();
            }
            // Mode détail
            var region = await _context.Region.FirstOrDefaultAsync(m => m.idRegion == id);
            if (region == null)
            {
                return NotFound();
            }
            Region = region;
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var regionDb = await _context.Region.FindAsync(Region.idRegion);
            if (regionDb == null) return NotFound();

            regionDb.intitule = Region.intitule;
            regionDb.latitude = Region.latitude;
            regionDb.longitude = Region.longitude;
            regionDb.totalPopulationRegion=Region.totalPopulationRegion;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var region = await _context.Region.FindAsync(id);
            if (region != null)
            {
                Region = region;
                _context.Region.Remove(Region);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostValidateAsync(int? id)
        {
            var regionDb = await _context.Region.FindAsync(id);
            if (regionDb == null) return NotFound();

            regionDb.etat = 5;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteSelectedAsync([FromBody] int[] ids)
        {
            var regions = _context.Region.Where(r => ids.Contains(r.idRegion));
            _context.Region.RemoveRange(regions);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostValidateSelectedAsync([FromBody] List<int> ids)
        {
            // if (ids == null || ids.Count == 0) return BadRequest();

            var regions = _context.Region.Where(r => ids.Contains(r.idRegion));
            foreach (var region in regions)
            {
                region.etat = 5; // ou autre valeur de validation
            }
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

    }
    
}
