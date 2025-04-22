using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Services;     
using Microsoft.AspNetCore.Hosting;     
using System.IO;
using Microsoft.AspNetCore.Http;
using Festisfeer.Presentation.ViewModels;  // Voeg dit toe om de ViewModel te importeren


namespace Festisfeer.Presentation.Controllers
{
    //Controller van de festivals
    public class FestivalController : Controller
    {
        private readonly FestivalService _festivalService;  // Verwijzing naar de logica laag
        private readonly IWebHostEnvironment _hostEnvironment; //voor de afbeeldingen

        //Constructor voor service en hostservice / afbeelding uploaden
        public FestivalController(FestivalService festivalService, IWebHostEnvironment hostEnvironment)
        {
            _festivalService = festivalService;
            _hostEnvironment = hostEnvironment;
        }

        //Toon alle festivals op de pagina
        public IActionResult Index()
        {
            var festivals = _festivalService.GetFestivals(); // Haalt festivals op via de festivalservice
            return View(festivals); //Naar de view sturen
        }

        //Toon alle festivals op de pagina 
        public IActionResult Past()
        {
            var festivals = _festivalService.GetFestivals();
            return View(festivals);
        }

        // Formulier om een nieuw festival toe te voegen
        public IActionResult Add()
        {
            //Rol pakken en kijken of het een beheerdr is voor de add pagina
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "beheerder")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new Festival()); 
        }

        //Toevoegen aan database method
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

                _festivalService.AddFestival(festival); //Service gebruiken om een festival toe te voegen 
                return RedirectToAction("Index"); //Terug naar index pagina
            }

            return View(festival);
        }

        //Details van een specefieke festival tonen
        public IActionResult Details(int id)
        {
            var festival = _festivalService.GetFestivalById(id); // Ophalen via service

            if (festival == null)
            {
                return NotFound(); //Error pagina 
            }

            return View(festival); //Details van een specefieke festival tonen
        }
    }
}