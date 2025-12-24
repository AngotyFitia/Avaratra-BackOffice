using Avaratra.BackOffice.Data;
using Avaratra.BackOffice.Models;
using BCrypt.Net;

namespace Avaratra.BackOffice.Services{

    public class AuthentificationService
    {
        private readonly Avaratra.BackOffice.Data.ApplicationDbContext _context;

        public AuthentificationService(Avaratra.BackOffice.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Utilisateur Login(string email, string motDePasse)
        {
            var user = _context.Utilisateur.FirstOrDefault(u => u.email == email);

            if (user == null)
            {
                Console.WriteLine("Utilisateur introuvable");
                return null;
            }

            Console.WriteLine($"Hash en base : {user.motDePasse}");
            Console.WriteLine($"Mot de passe saisi : {motDePasse}");

            bool isValid = BCrypt.Net.BCrypt.Verify(motDePasse, user.motDePasse);

            Console.WriteLine($"Résultat de la vérification : {isValid}");

            return user;
        }

    }
} 

