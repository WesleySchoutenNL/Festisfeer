using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;

namespace Festisfeer.Presentation.Controllers
{
    public class FestivalController : Controller
    {
        private readonly IFestivalRepository _festivalRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        // Constructor
        public FestivalController(IFestivalRepository festivalRepository, IWebHostEnvironment hostEnvironment)
        {
            _festivalRepository = festivalRepository;
            _hostEnvironment = hostEnvironment;
        }

        // Alle festivals weergeven
        public IActionResult Index()
        {
            var festivals = _festivalRepository.GetFestivals();  // Haal alle festivals via de interface
            return View(festivals);  // Stuur ze naar de view
        }

        // Festival toevoegen (form)
        public IActionResult Add()
        {
            return View(new Festival());
        }

        // Voeg festival toe aan database
        [HttpPost]
        public IActionResult Add(Festival festival, Microsoft.AspNetCore.Http.IFormFile festivalImg)
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

                _festivalRepository.AddFestival(festival);  // Voeg festival toe via de interface
                return RedirectToAction("Index");
            }

            return View(festival);
        }

        // Festival details weergeven
        public IActionResult Details(int id)
        {
            var festival = _festivalRepository.GetFestivalById(id);  // Haal specifiek festival via de interface

            if (festival == null)
            {
                return NotFound();  // Foutmelding als festival niet bestaat
            }

            return View(festival);  // Stuur festival naar de detailspagina
        }
    }
}