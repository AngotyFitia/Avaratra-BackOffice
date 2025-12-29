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
builder.Services.AddScoped<Avaratra.BackOffice.Services.AuthentificationService>();

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
app.UseAuthentication(); 

app.UseAuthorization();

app.MapRazorPages();

app.Run();
