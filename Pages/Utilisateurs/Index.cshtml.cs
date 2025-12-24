using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Avaratra.BackOffice.Data;
using Avaratra.BackOffice.Models;
using Avaratra.BackOffice.Services;

using System.Security.Claims; 
using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authentication.Cookies;

using System.ComponentModel.DataAnnotations;    

namespace Avaratra.BackOffice.Pages_Utilisateurs
{
    public class IndexModel : PageModel
    {
        private readonly AuthentificationService _authService;
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public IndexModel(Avaratra.BackOffice.Data.ApplicationDbContext context, AuthentificationService authService)
        {
            _context = context;
            _authService = authService;
        }

        public IList<Utilisateur> Utilisateur { get;set; } = default!;
        
        [BindProperty] 
        [Required(ErrorMessage = "L'adresse email est obligatoire.")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        public string MotDePasse { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Utilisateur != null)
            {
                Utilisateur = await _context.Utilisateur.ToListAsync();
            }
            ViewData["Layout"] = "_LayoutLogin";
            ViewData["BackgroundImage"] = "/assets/images/background.png";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { ViewData["Layout"] = "_LayoutLogin"; 
                ViewData["BackgroundImage"] = "/assets/images/background.png"; 
                return Page(); // garder les erreurs 
            }
            Utilisateur user=_authService.Login(Email, MotDePasse);
            if(user!=null)
            {
                // Pour la création un cookie d’authentification
                // var claims = new List<Claim> { new Claim(ClaimTypes.Name, Email) };
                var userJson = System.Text.Json.JsonSerializer.Serialize(user);
                var claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name, Email), 
                    new Claim("UserData", userJson)};
                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("login", principal);
                return Redirect("/");
            }
            TempData["ErrorMessage"] = "Email ou mot de passe incorrect.";
            return RedirectToPage("/Utilisateurs/Index");
        }
    }
}
