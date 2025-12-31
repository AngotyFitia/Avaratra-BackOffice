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
        public Region Region { get; set; } = default!; //une seule region

        [BindProperty]
        public List<int> SelectedIds { get; set; } = new();

        // recherche avancée 
        [BindProperty(SupportsGet = true)]
        public string? SearchIntitule { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinPopulation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxPopulation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Etat { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 2;// valeur par défaut


        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            Region.etat = 0;
            Region.totalPopulationRegion=0;
            _context.Region.Add(Region);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
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
            if (regionDb == null) return NotFound();

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
                region.etat = 5; // validé
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

            var importer = new CsvImporter<Region>(RegionCsvMapperService.Map);
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
            TempData["Succes"] = "Import terminé avec succès.";
            return RedirectToPage();
        }


        public async Task<List<Region>> GetRegionsValideesAsync() { 
            return await _context.Region 
                .Where(r => r.etat == 5) .OrderBy(r => r.intitule) .ToListAsync(); 
        }


       public async Task<IActionResult> OnGetExportPdfAsync(int id)
        {
            var region = await _context.Region
                .Include(r => r.Districts)
                .FirstOrDefaultAsync(r => r.idRegion == id);

            if (region == null)
                return NotFound();

            var districts = region.Districts
                .Select(d => (d.intitule, d.totalPopulationDistrict))
                .ToList();

            var pdfBytes = PdfReportGeneratorService.GenerateRegionReport(region.intitule, region.totalPopulationRegion, districts);

            return File(pdfBytes, "application/pdf", $"Region_{region.intitule}.pdf");
        }

        public async Task<IActionResult> OnGetExportAllPdfAsync(string search)
        {
            // Récupérer les régions filtrées selon la recherche
            var query = _context.Region.Include(r => r.Districts).AsQueryable();
            var regions = await query.ToListAsync();

            if (!regions.Any())
                return NotFound();

            // Générer le PDF
            var pdfBytes = PdfReportGeneratorService.GenerateRegionsListReport(regions);

            return File(pdfBytes, "application/pdf", "Regions_Report.pdf");
        }

    }
    
}
