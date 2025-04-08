using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Voeg session services toe
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Sessie tijdsduur instellen
});

// Voeg services toe aan de container
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FestivalRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IFestivalRepository, FestivalRepository>();
builder.Services.AddScoped<FestivalService>();
    
var app = builder.Build();

// Zorg ervoor dat de session middleware vóór routing wordt geplaatst
app.UseSession();

// Configureer de HTTP-aanvraagpijplijn
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();