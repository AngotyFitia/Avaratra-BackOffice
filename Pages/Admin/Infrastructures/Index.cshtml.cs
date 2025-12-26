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

namespace Avaratra.BackOffice.Pages_Admin_Infrastructures
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<Infrastructure> Infrastructures { get; set; } = default!;
        public List<Categorie> CategoriesValidees { get; set; } = new();

        [BindProperty]
        public Infrastructure Infrastructure { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchInfrastructure { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchCategorie { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Etat { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
            const int pageSize = 2;
            var query = _context.Infrastructure
                                .Include(d => d.Categorie)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchInfrastructure))
                query = query.Where(r => r.intitule.Contains(SearchInfrastructure));
            if (!string.IsNullOrWhiteSpace(SearchCategorie))
                query = query.Where(r => r.Categorie.intitule.Contains(SearchCategorie));
            if (Etat.HasValue)
                query = query.Where(r => r.etat == Etat.Value);
            query = query.OrderBy(r => r.intitule);
            Infrastructures = await PaginatedList<Infrastructure>.CreateAsync(query, pageIndex ?? 1, pageSize);

            CategoriesValidees = await _context.Categorie
                                            .Where(r => r.etat == 5)
                                            .OrderBy(r => r.intitule)
                                            .ToListAsync();
            }
            var infrastructure = await _context.Infrastructure.FirstOrDefaultAsync(m => m.idInfrastructure == id);

            Infrastructure=infrastructure;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            Console.WriteLine(Infrastructure.IdCategorie);
            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            _context.Infrastructure.Add(Infrastructure);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

         public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var infrastructureDb = await _context.Infrastructure.FindAsync(Infrastructure.idInfrastructure);
            if (infrastructureDb == null) return NotFound();
            infrastructureDb.intitule = Infrastructure.intitule;
            infrastructureDb.IdCategorie = Infrastructure.IdCategorie;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var infrastructure = await _context.Infrastructure.FindAsync(id);
            if (infrastructure != null)
            {
                Infrastructure = infrastructure;
                _context.Infrastructure.Remove(Infrastructure);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostValidateAsync(int? id)
        {
           var infrastructureDb = await _context.Infrastructure.FindAsync(id);
            infrastructureDb.etat = 5;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
