using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Avaratra.BackOffice.Data;
using Avaratra.BackOffice.Utils;
using Avaratra.BackOffice.Models;

namespace Avaratra.BackOffice.Pages_Admin_TypeMesures
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<TypeMesure> TypeMesures { get; set; } = default!;
        public List<Unite> UnitesValidees { get; set; } = new();

        [BindProperty]
        public TypeMesure TypeMesure { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchTypeMesure { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchDescription { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchUnite { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Etat { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
            const int pageSize = 2;
            var query = _context.TypeMesure
                                .Include(d => d.Unite)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTypeMesure))
                query = query.Where(r => r.intitule.Contains(SearchTypeMesure));
            if (!string.IsNullOrWhiteSpace(SearchDescription))
                query = query.Where(r => r.description.Contains(SearchDescription));
            if (!string.IsNullOrWhiteSpace(SearchUnite))
                query = query.Where(r => r.Unite.symbole.Contains(SearchUnite));
            if (Etat.HasValue)
                query = query.Where(r => r.etat == Etat.Value);
            query = query.OrderBy(r => r.intitule);
            TypeMesures = await PaginatedList<TypeMesure>.CreateAsync(query, pageIndex ?? 1, pageSize);

            UnitesValidees = await _context.Unite
                                            .Where(r => r.etat == 5)
                                            .OrderBy(r => r.symbole)
                                            .ToListAsync();
            }
            var typeMesure = await _context.TypeMesure.FirstOrDefaultAsync(m => m.idTypeMesure == id);

            TypeMesure=typeMesure;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Console.WriteLine(TypeMesure.IdUnite);
            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            _context.TypeMesure.Add(TypeMesure);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var typeMesureDb = await _context.TypeMesure.FindAsync(TypeMesure.idTypeMesure);
            if (typeMesureDb == null) return NotFound();
            typeMesureDb.intitule = TypeMesure.intitule;
            typeMesureDb.description = TypeMesure.description;
            typeMesureDb.IdUnite = TypeMesure.IdUnite;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var typeMesure = await _context.TypeMesure.FindAsync(id);
            if (typeMesure != null)
            {
                TypeMesure = typeMesure;
                _context.TypeMesure.Remove(TypeMesure);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostValidateAsync(int? id)
        {
           var typeMesureDb = await _context.TypeMesure.FindAsync(id);
            typeMesureDb.etat = 5;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
