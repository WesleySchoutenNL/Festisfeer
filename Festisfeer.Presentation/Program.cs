using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Voeg services toe aan de container
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FestivalRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IFestivalRepository, FestivalRepository>();


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
app.UseStaticFiles(); 


app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start de applicatie
app.Run();