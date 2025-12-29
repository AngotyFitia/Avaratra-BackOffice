using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Avaratra.BackOffice.Data;
using Avaratra.BackOffice.Models;
using BCrypt.Net;

namespace Avaratra.BackOffice.Pages_Utilisateurs
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string MotDePasse { get; set; }

        public void OnGet()
        {
            ViewData["Layout"] = "_LayoutLogin";
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(MotDePasse))
            {
                ModelState.AddModelError(string.Empty, "Email et mot de passe sont obligatoires.");
                return Page();
            }

            var user = new Utilisateur
            {
                IdProfil=1,
                email = Email,
                motDePasse = BCrypt.Net.BCrypt.HashPassword(MotDePasse), // hash avant insertion
                IdCommune=1
            };

            _context.Utilisateur.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Compte créé avec succès. Vous pouvez vous connecter.";
            return RedirectToPage("/Utilisateurs/Index");
        }
    }
}
