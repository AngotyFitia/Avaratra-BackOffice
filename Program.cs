using Avaratra.BackOffice.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddRazorPages();

// Dependency Injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    ));

// pour l'authentification
builder.Services.AddScoped<Avaratra.BackOffice.Services.AuthentificationService>();

// pour les exceptions
builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        var p = options.ModelBindingMessageProvider;

        // Un paramètre est requis ici (même s’il n’est pas utilisé)
        p.SetValueMustNotBeNullAccessor(_ => "");

        // Ces accesseurs acceptent un paramètre (la valeur tentée)
        p.SetValueIsInvalidAccessor(value => $"La valeur '{value}' est invalide.");
        p.SetValueMustBeANumberAccessor(value => $"La valeur '{value}' doit être un nombre.");

        // Celui-ci accepte le nom du champ manquant
        p.SetMissingBindRequiredValueAccessor(fieldName => $"Le champ '{fieldName}' est requis.");

        // Optionnel selon version (si disponible) — 2 paramètres: valeur + champ
        // p.SetAttemptedValueIsInvalidAccessor((value, fieldName) => $"La valeur '{value}' pour '{fieldName}' est invalide.");
    });


builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    var cultures = new[] { new CultureInfo("fr-FR") };
    opts.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("fr-FR");
    opts.SupportedCultures = cultures;
    opts.SupportedUICultures = cultures;
});


// pour les cookies d'authentification
builder.Services.AddAuthentication("login")
    .AddCookie("login", options =>
    {
        options.LoginPath = "/Utilisateurs/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // expiration apres 30 minute
        // renouvellement automatique c'est-à-dire: 
        // si activé, chaque requête renouvelle le cookie et repousse l’expiration tant que l’utilisateur est actif.
        options.SlidingExpiration = true; 
    });

var app = builder.Build();

// pour le seeding auto de la base
using (var scope = app.Services.CreateScope()) { 
    var services = scope.ServiceProvider; 
    SeedData.Initialize(services); 
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapDefaultControllerRoute();
app.UseAuthentication(); 

app.UseAuthorization();

app.MapRazorPages();

app.Run();
