using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Services;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Presentation.Services;

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
builder.Services.AddScoped<FestivalRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IFestivalRepository, FestivalRepository>();
builder.Services.AddScoped<FestivalService>();

// Session-related services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Let op: voor UseAuthorization!
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();