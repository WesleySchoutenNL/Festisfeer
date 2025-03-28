using Microsoft.AspNetCore.Mvc;
using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Festisfeer.Presentation.Controllers
{
    public class FestivalController : Controller
    {
        private readonly FestivalRepository _festivalRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        public FestivalController(FestivalRepository festivalRepository, IWebHostEnvironment hostEnvironment)
        {
            _festivalRepository = festivalRepository;
            _hostEnvironment = hostEnvironment;
        }

        // Actie om festivals weer te geven
        public IActionResult Index()
        {
            var festivals = _festivalRepository.GetFestivals(); // Haal alle festivals op
            return View(festivals); // Geef de lijst van festivals door naar de view
        }

        // Actie om het formulier voor het toevoegen van een festival te tonen
        public IActionResult Add()
        {
            // Geef een leeg Festival object door naar de view
            return View(new Festival());  // Dit zorgt ervoor dat het model niet null is
        }

        // Actie om het festival toe te voegen (POST)
        [HttpPost]
        public IActionResult Add(Festival festival, Microsoft.AspNetCore.Http.IFormFile festivalImg)
        {
            if (ModelState.IsValid)
            {
                if (festivalImg != null)
                {
                    // Opslaan van het bestand
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", festivalImg.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        festivalImg.CopyTo(fileStream);
                    }

                    // Zet de URL van het bestand in het model
                    festival.FestivalImg = "/images/" + festivalImg.FileName;
                }

                _festivalRepository.AddFestival(festival);  // Voeg festival toe aan de database
                return RedirectToAction("Index"); // Terug naar de lijst van festivals
            }

            return View(festival); // Als er een fout is, toon het formulier opnieuw
        }
    }
}