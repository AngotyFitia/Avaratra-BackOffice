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

namespace Avaratra.BackOffice.Pages_Admin_Unites
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public PaginatedList<Unite> Unites { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchUnity { get; set; }

        [BindProperty]
        public Unite Unite { get; set; } = default!; 

        [BindProperty(SupportsGet = true)]
        public int? Etat { get; set; }   

        public async Task<IActionResult> OnGetAsync(int? id, int? pageIndex)
        {
            if (id == null)
            {
                const int pageSize = 2;
                var query = _context.Unite.AsQueryable();
                if (!string.IsNullOrWhiteSpace(SearchUnity))
                    query = query.Where(r => r.symbole.Contains(SearchUnity));

                if (Etat.HasValue)
                    query = query.Where(c => c.etat == Etat.Value);

                query = query.OrderBy(r => r.symbole);
                Unites = await PaginatedList<Unite>.CreateAsync(query, pageIndex ?? 1, pageSize);

            }
            var unite = await _context.Unite.FirstOrDefaultAsync(m => m.idUnite == id);

            Unite= unite;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid){
                ViewData["ShowCreateModal"] = true;
                return Page();
            }
            Unite.etat = 0;
            _context.Unite.Add(Unite);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {   
            var uniteDb = await _context.Unite.FindAsync(Unite.idUnite);
            if (uniteDb == null) return NotFound();
            uniteDb.symbole = Unite.symbole;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            var unite = await _context.Unite.FindAsync(id);
            if (unite != null)
            {
                Unite = unite;
                _context.Unite.Remove(Unite);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostValidateAsync(int? id)
        {
            var uniteDb = await _context.Unite.FindAsync(id);

            uniteDb.etat = 5;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
