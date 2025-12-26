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

namespace Avaratra.BackOffice.Pages_Admin_Categories
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public PaginatedList<Categorie> Categories { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchCategory { get; set; }

        [BindProperty]
        public Categorie Categorie { get; set; } = default!; 

        [BindProperty(SupportsGet = true)]
        public int? Etat { get; set; }   

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
                const int pageSize = 2;
                var query = _context.Categorie.AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchCategory))
                    query = query.Where(r => r.intitule.Contains(SearchCategory));

                if (Etat.HasValue)
                    query = query.Where(c => c.etat == Etat.Value);

                query = query.OrderBy(r => r.intitule);
                Categories = await PaginatedList<Categorie>.CreateAsync(query, pageIndex ?? 1, pageSize);

            }
            var categorie = await _context.Categorie.FirstOrDefaultAsync(m => m.idCategorie == id);

            Categorie= categorie;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            Categorie.etat = 0;
            _context.Categorie.Add(Categorie);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var categorieDb = await _context.Categorie.FindAsync(Categorie.idCategorie);
            if (categorieDb == null) return NotFound();
            categorieDb.intitule = Categorie.intitule;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var categorie = await _context.Categorie.FindAsync(id);
            if (categorie != null)
            {
                Categorie = categorie;
                _context.Categorie.Remove(Categorie);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostValidateAsync(int? id)
        {
            var categorieDb = await _context.Categorie.FindAsync(id);

            categorieDb.etat = 5;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
