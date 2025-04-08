using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Festisfeer.Presentation.Controllers
{
    public class FestivalController : Controller
    {
        private readonly FestivalService _festivalService;  // Gebruik de service
        private readonly IWebHostEnvironment _hostEnvironment;

        // Constructor
        public FestivalController(FestivalService festivalService, IWebHostEnvironment hostEnvironment)
        {
            _festivalService = festivalService;
            _hostEnvironment = hostEnvironment;
        }

        // Alle festivals weergeven
        public IActionResult Index()
        {
            var festivals = _festivalService.GetFestivals();  // Haal festivals via de service
            return View(festivals);  // Stuur ze naar de view
        }

        // Festival toevoegen (form)
        public IActionResult Add()
        {
            // Controleer of de gebruiker ingelogd is en de rol "beheerder" heeft
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "beheerder")  // Alleen beheerders mogen festivals toevoegen
            {
                return RedirectToAction("Index", "Home");  // Redirect naar de homepagina als de gebruiker geen beheerder is
            }

            return View(new Festival());
        }

        // Voeg festival toe aan database
        [HttpPost]
        public IActionResult Add(Festival festival, IFormFile festivalImg)
        {
            if (ModelState.IsValid)
            {
                if (festivalImg != null)
                {
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", festivalImg.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        festivalImg.CopyTo(fileStream);
                    }

                    festival.FestivalImg = "/images/" + festivalImg.FileName;
                }

                _festivalService.AddFestival(festival);  // Voeg festival toe via de service
                return RedirectToAction("Index");
            }

            return View(festival);
        }

        // Festival details weergeven
        public IActionResult Details(int id)
        {
            var festival = _festivalService.GetFestivalById(id);  // Haal festival via de service

            if (festival == null)
            {
                return NotFound();  // Foutmelding als festival niet bestaat
            }

            return View(festival);  // Stuur festival naar de detailspagina
        }
    }
}