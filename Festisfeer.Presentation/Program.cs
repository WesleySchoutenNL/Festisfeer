using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;  // Zorg ervoor dat deze namespace wordt ge�mporteerd

var builder = WebApplication.CreateBuilder(args);

// Voeg session services toe
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Sessie tijdsduur instellen
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Voeg services toe aan de container
builder.Services.AddControllersWithViews();

// Registratie van je repositories
builder.Services.AddScoped<IFestivalRepository, FestivalRepository>();  // Je interface en implementatie
builder.Services.AddScoped<IUserRepository, UserRepository>();  // Hier wordt de interface IUserRepository geregistreerd, niet UserRepository direct

// Registratie van services
builder.Services.AddScoped<FestivalService>();  // FestivalService wordt toegevoegd via DI
builder.Services.AddScoped<IUserSessionService, UserSessionService>();  // UserSessionService wordt toegevoegd via DI

// Session-gerelateerde services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();  // Zorg ervoor dat IHttpContextAccessor correct is geregistreerd

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Zorg ervoor dat session middleware v��r authorization wordt aangeroepen
app.UseAuthorization();

// Route configuratie
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();