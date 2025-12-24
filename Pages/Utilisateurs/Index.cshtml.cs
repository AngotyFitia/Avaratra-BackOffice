using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Avaratra.BackOffice.Data;
using Avaratra.BackOffice.Models;

namespace Avaratra.BackOffice.Pages_Utilisateurs
{
    public class IndexModel : PageModel
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Utilisateur> Utilisateur { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Utilisateur != null)
            {
                Utilisateur = await _context.Utilisateur.ToListAsync();
            }
            ViewData["Layout"] = "_LayoutLogin";
            ViewData["BackgroundImage"] = "/assets/images/background.png";
        }
    }
}
