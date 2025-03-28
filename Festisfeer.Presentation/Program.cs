using Festisfeer.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Voeg services toe aan de container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FestivalRepository>();

var app = builder.Build();

// Configureer de HTTP-aanvraagpijplijn.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Voeg dit toe om statische bestanden zoals afbeeldingen te serveren

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();