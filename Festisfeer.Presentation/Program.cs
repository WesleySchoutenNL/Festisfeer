using Festisfeer.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Voeg services toe aan de container
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FestivalRepository>();

var app = builder.Build();

// Configureer de HTTP-aanvraagpijplijn.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Voeg dit toe om statische bestanden zoals afbeeldingen te serveren

// De volgorde van de middleware is belangrijk
app.UseRouting(); // Routing middleware toevoegen vóór UseEndpoints

app.UseAuthorization(); // Als je autorisatie gebruikt

// Configureer de eindpunten voor controllers en routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start de applicatie
app.Run();