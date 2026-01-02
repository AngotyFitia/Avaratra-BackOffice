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
using Avaratra.BackOffice.Services.Exporting;
using Avaratra.BackOffice.Services.Importing;

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

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 2;

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
            const int pageSize = 2;
            var query = _context.District
                                .Include(d => d.Region)
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
            Districts = await PaginatedList<District>.CreateAsync(query, pageIndex ?? 1, PageSize);

            // Récupération des régions validées
            RegionsValidees = await _context.Region
                                            .Where(r => r.etat == 5)
                                            .OrderBy(r => r.intitule)
                                            .ToListAsync();
            }
            return Page();
        }

        public JsonResult OnGetCommunes(int id)
        {
            var communes = _context.Commune
                .Where(c => c.IdDistrict == id)
                .Select(c => new { c.idCommune, c.intitule, c.latitude, c.longitude,c.nombrePopulation})
                .ToList();
            return new JsonResult(communes);
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Console.WriteLine(District.IdRegion);
            if (!ModelState.IsValid){
                RegionsValidees = await _context.Region
                                            .Where(r => r.etat == 5)
                                            .OrderBy(r => r.intitule)
                                            .ToListAsync();
                ViewData["ShowCreateModal"] = true;
                return Page();
            }else{
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                District.geometrie = geometryFactory.CreatePoint(new Coordinate(0, 0));            
                District.totalPopulationDistrict = 0;
                District.etat = 0;
                _context.District.Add(District);
                TempData["Succes"] = $"Nouveau district ajouté: District {District.intitule}.";
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
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
                .Include(d => d.Communes) // jointure pour prendre les communes liées
                .FirstOrDefaultAsync(d => d.idDistrict == id);

            if (districtDb == null) return NotFound();
            districtDb.etat = 10;

            // La géométrie du district à partir des communes
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            // récupération des points des communes
            var communePoints = districtDb.Communes
                .Select(c => geometryFactory.CreatePoint(new Coordinate((double)c.longitude, (double)c.latitude)))
                .ToArray();

            if (communePoints.Length > 0)
            {
                // Création d'un multiPoint puis un polygone englobant
                var multiPoint = geometryFactory.CreateMultiPoint(communePoints);

                // polygone englobant toutes les communes
                districtDb.geometrie = multiPoint.ConvexHull(); 
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
                TempData["Erreur"] = "Aucun fichier sélectionné.";
                return RedirectToPage();
            }
            if (Path.GetExtension(csvFile.FileName).ToLower() != ".csv")
            {
                TempData["Erreur"] = "Le fichier doit être au format CSV.";
                return RedirectToPage();
            }
            var importer = new CsvImporter<District>(DistrictService.Map);
            var (districts, errors) = await importer.ImportAsync(csvFile.OpenReadStream());

            foreach (var district in districts){
                var region = await _context.Region.FirstOrDefaultAsync(r => r.intitule == district.TagRegion);
                if(region == null){
                    errors.Add($"Région: '{district.TagRegion}' absente. Importez les régions avant les districts.");
                    continue;
                }
                bool existDistrict = await _context.District
                                    .AnyAsync(d => d.intitule == district.intitule && d.IdRegion == region.idRegion);
                if(!existDistrict){
                    district.IdRegion = region.idRegion;
                    _context.District.Add(district);
                }else{
                    errors.Add($"District: 'Le {district.intitule} de la région {region.intitule}' existe déjà.");
                }
            }
            if (errors.Any())
            {
                TempData["Erreur"] = string.Join("<br/>", errors);
                return RedirectToPage();
            }
            await _context.SaveChangesAsync();
            string districtWord = districts.Count <= 1 ? "district" : "districts";
            string districtVerb = districts.Count <= 1 ? "importé" : "importés";
            string errorWord = errors.Count <= 1 ? "erreur" : "erreurs";
            TempData["Succes"] = $"Import terminé. {districts.Count} {districtWord} {districtVerb}, {errors.Count} {errorWord}.";
            return RedirectToPage();
        }
        
       public async Task<IActionResult> OnGetExportPdfAsync(int id)
        {
            var district = await _context.District
                .Include(d => d.Communes)
                .FirstOrDefaultAsync(d => d.idDistrict == id);

            if (district == null)
                return NotFound();

            // Liste des communes avec leur population
            var communes = district.Communes
                .Select(c => (c.intitule, c.nombrePopulation))
                .ToList();
            var totalPopulationDistrict = communes.Sum(c => c.nombrePopulation);
            var pdfBytes = PdfReportService.GenerateEntityReport(
                "District",
                district.intitule,
                totalPopulationDistrict,
                communes,
                "commune"
            );

            return File(pdfBytes, "application/pdf", $"District_{district.intitule}.pdf");
        }

        public async Task<IActionResult> OnGetExportAllPdfAsync(string SearchIntitule, string searchRegion, int? MinPopulation, int? MaxPopulation, int? Etat)
        {   
            var query = _context.District
                .Include(d => d.Communes)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchIntitule))
            {
                query = query.Where(d => d.intitule.Contains(SearchIntitule));
            }

            if (!string.IsNullOrWhiteSpace(searchRegion))
            {
                query = query.Where(d => d.Region.intitule.Contains(searchRegion));
            }

            if (MinPopulation.HasValue)
            {
                query = query.Where(d => d.Communes.Sum(c => c.nombrePopulation) >= MinPopulation.Value);
            }

            if (MaxPopulation.HasValue)
            {
                query = query.Where(d => d.Communes.Sum(c => c.nombrePopulation) <= MaxPopulation.Value);
            }

            if (Etat.HasValue)
            {
                query = query.Where(d => d.Communes.Count >= Etat.Value);
            }

            var districts = await query.ToListAsync();
            if (!districts.Any())
                return NotFound();

            var pdf = PdfReportService.GenerateEntitiesListReport(
                "district",
                districts,
                d => d.intitule,
                d => d.Communes.Sum(c => c.nombrePopulation),
                d => d.Communes.Count,
                "communes"
            );
            return File(pdf, "application/pdf", "Districts_Report.pdf");
        }
    }
}
