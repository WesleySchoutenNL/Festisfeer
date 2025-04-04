using Microsoft.AspNetCore.Mvc;
using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace Festisfeer.Presentation.Controllers
{
    public class FestivalController : Controller
    {
        private readonly FestivalRepository _festivalRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        // Constructor: zorgt dat we toegang hebben tot de database en de serveromgeving
        public FestivalController(FestivalRepository festivalRepository, IWebHostEnvironment hostEnvironment)
        {
            _festivalRepository = festivalRepository;
            _hostEnvironment = hostEnvironment;
        }

        // Laat alle festivals zien op de homepage
        public IActionResult Index()
        {
            var festivals = _festivalRepository.GetFestivals(); // Haalt alle festivals uit de database
            return View(festivals); // Stuurt ze door naar de festival pagina
        }

        //Formulier laten zien om nieuwe festivals toe te voegen
        public IActionResult Add()
        {
            return View(new Festival()); // Stuurt een leeg festivalmodel naar de pagina
        }

        //Formulier verwerken vooor een nieuw festival
        [HttpPost]
        public IActionResult Add(Festival festival, Microsoft.AspNetCore.Http.IFormFile festivalImg)
        {
            if (ModelState.IsValid) 
            {
                if (festivalImg != null)
                {
                    // Slaat de afbeelding op in de map wwwroot/images
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", festivalImg.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        festivalImg.CopyTo(fileStream); // Kopieert de afbeelding naar de server
                    }

                    // Zet de link naar de afbeelding in het model
                    festival.FestivalImg = "/images/" + festivalImg.FileName;
                }

                _festivalRepository.AddFestival(festival); // Slaat het festival op in de database
                return RedirectToAction("Index"); // Gaat terug naar de hoofdpagina
            }

            return View(festival); // Als er iets mis is, laat het formulier opnieuw zien
        }

        // Laat details van één specifiek festival zien voor op de detail pagina
        public IActionResult Details(int id)
        {
            var festival = _festivalRepository.GetFestivals().FirstOrDefault(f => f.Id == id); // Zoek het festival met de juiste ID

            if (festival == null)
            {
                return NotFound(); // Als het festival niet bestaat, geef een foutmelding (404)
            }

            return View(festival); // Stuur het gevonden festival naar de detailpagina
        }
    }
}