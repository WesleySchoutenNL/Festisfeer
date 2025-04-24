using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Festisfeer.Presentation.ViewModels;  // ViewModel import
using System.Linq;  // Voor .Select()

namespace Festisfeer.Presentation.Controllers
{
    // Controller van de festivals
    public class FestivalController : Controller
    {
        private readonly FestivalService _festivalService;  // Verwijzing naar de domain (Logica) laag
        private readonly IWebHostEnvironment _hostEnvironment; // Voor de afbeeldingen

        // Constructor voor service en hostservice / afbeelding uploaden
        public FestivalController(FestivalService festivalService, IWebHostEnvironment hostEnvironment)
        {
            _festivalService = festivalService;
            _hostEnvironment = hostEnvironment;
        }

        // Toon alle festivals op de pagina
        public IActionResult Index()
        {
            var festivals = _festivalService.GetFestivals();

            var viewModels = festivals.Select(f => new FestivalViewModel
            {
                Id = f.Id,
                Name = f.Name,
                Location = f.Location,
                Period = $"{f.StartDateTime:dd MMM yyyy} - {f.EndDateTime:dd MMM yyyy}",
                Genre = f.Genre,
                FestivalImg = f.FestivalImg,
                TicketPriceFormatted = $"€ {f.TicketPrice:0.00}"
            }).ToList();

            return View(viewModels);
        }

        // Toon alle festivals op de pagina (bijv. alleen de oude)
        public IActionResult Past()
        {
            var festivals = _festivalService.GetFestivals();
            return View(festivals);
        }

        // Formulier om een nieuw festival toe te voegen
        public IActionResult Add()
        {
            // Rol pakken en kijken of het een beheerder is
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "beheerder")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new Festival());
        }

        // Toevoegen aan database
        [HttpPost]
        public IActionResult Add(Festival festival, IFormFile festivalImg)
        {
            if (ModelState.IsValid)
            {
                // Als er een afbeelding is meegegeven, sla die dan op
                if (festivalImg != null)
                {
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", festivalImg.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        festivalImg.CopyTo(fileStream);
                    }

                    // Koppel het pad van de afbeelding aan het festival
                    festival.FestivalImg = "/images/" + festivalImg.FileName;
                }

                _festivalService.AddFestival(festival); // Service gebruiken om festival toe te voegen
                return RedirectToAction("Index");
            }

            return View(festival);
        }

        // Details van een specifiek festival tonen
        public IActionResult Details(int id)
        {
            var festival = _festivalService.GetFestivalById(id);

            if (festival == null)
            {
                return NotFound();
            }

            var viewModel = new FestivalViewModel
            {
                Id = festival.Id,
                Name = festival.Name,
                Location = festival.Location,
                Period = $"{festival.StartDateTime:dd MMM yyyy} - {festival.EndDateTime:dd MMM yyyy}",
                Genre = festival.Genre,
                FestivalImg = festival.FestivalImg,
                TicketPriceFormatted = $"€ {festival.TicketPrice:0.00}"
            };

            return View(viewModel);
        }
    }
}