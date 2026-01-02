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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Avaratra.BackOffice.Utils;
using Avaratra.BackOffice.Services;

namespace Avaratra.BackOffice.Pages_Regions
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;
        public PaginatedList<Region> Regions { get; set; } = default!;

        [BindProperty]
        public Region Region { get; set; } = default!; 

        [BindProperty]
        public List<int> SelectedIds { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchIntitule { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinPopulation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxPopulation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Etat { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 2;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }else{
                Region.etat = 0;
                Region.totalPopulationRegion=0;
                _context.Region.Add(Region);
                TempData["Succes"] = $"Ajout terminé: Région {Region.intitule}.";
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
                var query = _context.Region.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(SearchIntitule))
                        query = query.Where(r => r.intitule.Contains(SearchIntitule));
                    if (MinPopulation.HasValue)
                        query = query.Where(r => r.totalPopulationRegion >= MinPopulation.Value);
                    if (MaxPopulation.HasValue)
                        query = query.Where(r => r.totalPopulationRegion <= MaxPopulation.Value);
                    if (Etat.HasValue)
                        query = query.Where(r => r.etat == Etat.Value);
                    Regions = await PaginatedList<Region>.CreateAsync(query, pageIndex ?? 1, PageSize);
                return Page();
            }
            // Mode détails
            var region = await _context.Region
                    .Include(r => r.Districts)
                    .FirstOrDefaultAsync(m => m.idRegion == id);
            if (region == null) return NotFound();
            Region = region;
            return Page();
        }

        public async Task<IActionResult> OnGetDistrictsAsync(int id)
        {
            var districts = await _context.District
                .Where(d => d.IdRegion == id)
                .Select(d => new { d.idDistrict, d.intitule })
                .ToListAsync();
            return new JsonResult(districts);
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var regionDb = await _context.Region.FindAsync(Region.idRegion);
            if (regionDb == null) return NotFound();
            regionDb.intitule = Region.intitule;
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
            regionDb.etat = 5;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteSelectedAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new { error = "Aucun ID reçu." });

            var regions = _context.Region.Where(r => ids.Contains(r.idRegion));
            _context.Region.RemoveRange(regions);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    
        public async Task<IActionResult> OnPostValidateSelectedAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new { error = "Aucun ID reçu." });

            var regions = _context.Region.Where(r => ids.Contains(r.idRegion));
            foreach (var region in regions)
            {
                region.etat = 5;
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostImportCsvAsync(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Erreur"] = "Aucun fichier sélectionné.";
                return Page();
            }

            if (Path.GetExtension(csvFile.FileName).ToLower() != ".csv")
            {
                TempData["Erreur"] = "Le fichier doit être au format CSV.";
                return Page();
            }

            var importer = new CsvImporter<Region>(RegionService.Map);
            var (regions, errors) = await importer.ImportAsync(csvFile.OpenReadStream());

            foreach (var region in regions)
            {
                if (!await _context.Region.AnyAsync(r => r.intitule == region.intitule))
                    _context.Region.Add(region);
                else
                    errors.Add($"Région '{region.intitule}' déjà existante.");
            }

            if (errors.Any())
            {
                TempData["Erreur"] = string.Join("<br/>", errors);
                return Page();
            }

            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
            string regionWord = regions.Count <= 1 ? "région" : "régions";
            string regionVerb = regions.Count <= 1 ? "importée" : "importées";
            string errorWord = errors.Count <= 1 ? "erreur" : "erreurs";
            TempData["Succes"] = $"Import terminé. {regions.Count} {regionWord} {regionVerb}, {errors.Count} {errorWord}.";
            return RedirectToPage();
        }

        public async Task<List<Region>> GetRegionsValideesAsync()
        { 
            return await _context.Region 
                .Where(r => r.etat == 5) .OrderBy(r => r.intitule) .ToListAsync(); 
        }

        public async Task<IActionResult> OnGetExportPdfAsync(int id)
        {
            var region = await _context.Region
                .Include(r => r.Districts)
                    .ThenInclude(d => d.Communes)
                .FirstOrDefaultAsync(r => r.idRegion == id);

            if (region == null)
                return NotFound();

            // Recalculer les populations des districts à partir des communes
            var districts = region.Districts
                .Select(d => (
                    d.intitule,
                    d.Communes?.Sum(c => c.nombrePopulation) ?? 0
                ))
                .ToList();


            var pdfBytes = PdfGenerator.GenerateEntityReport(
                "Région",
                region.intitule,
                region.totalPopulationRegion ?? 0,
                districts,
                "Nom du district"
            );

            return File(pdfBytes, "application/pdf", $"Region_{region.intitule}.pdf");
        }

        public async Task<IActionResult> OnGetExportAllPdfAsync(string SearchIntitule, int? MinPopulation, int? MaxPopulation, int? Etat)
        {   
            var query = _context.Region
                .Include(r => r.Districts)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(SearchIntitule))
            {
                query = query.Where(r => r.intitule.Contains(SearchIntitule));
                Console.WriteLine("misy search");
            }
            if (MinPopulation.HasValue)
            {
                query = query.Where(r => r.Districts.Sum(d => d.Communes.Sum(c => c.nombrePopulation)) >= MinPopulation.Value);
            }
            if (MaxPopulation.HasValue)
            {
                query = query.Where(r => r.Districts.Sum(d => d.Communes.Sum(c => c.nombrePopulation)) <= MaxPopulation.Value);
            }
            if (Etat.HasValue)
            {
                query = query.Where(r => r.Districts.Count >= Etat.Value);
            }
            var regions = await query.ToListAsync();
            if (!regions.Any())
                return NotFound();

            var pdf = PdfGenerator.GenerateEntitiesListReport(
                "région",
                regions,
                r => r.intitule,
                r => r.Districts.Sum(d => d.Communes.Sum(c => c.nombrePopulation)),
                r => r.Districts.Count,
                "districts"
            );

            return File(pdf, "application/pdf", "Regions_Report.pdf");
        }

    }
    
}
