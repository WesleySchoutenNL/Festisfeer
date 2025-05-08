using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Festisfeer.Presentation.ViewModels;  // ViewModel import

namespace Festisfeer.Presentation.Controllers
{
    // Controller voor de festivals
    public class FestivalController : Controller
    {
        private readonly FestivalService _festivalService;  // Verwijzing naar de domain (logica) laag
        private readonly IWebHostEnvironment _hostEnvironment; // Voor de afbeeldingen
        private readonly ReviewService _reviewService; // Service voor reviews

        // Constructor voor services en hostomgeving / afbeelding uploaden
        public FestivalController(FestivalService festivalService, ReviewService reviewService, IWebHostEnvironment hostEnvironment)
        {
            _festivalService = festivalService;
            _reviewService = reviewService;
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

        // Details van een specifiek festival tonen
        public IActionResult Details(int id)
        {
            var festival = _festivalService.GetFestivalById(id);

            if (festival == null)
            {
                return NotFound();
            }

            // Haal de reviews voor dit festival op
            var reviews = _reviewService.GetReviewsByFestivalId(id);

            // Maak het ViewModel aan en voeg reviews toe
            var viewModel = new FestivalViewModel
            {
                Id = festival.Id,
                Name = festival.Name,
                Location = festival.Location,
                Period = $"{festival.StartDateTime:dd MMM yyyy} - {festival.EndDateTime:dd MMM yyyy}",
                Genre = festival.Genre,
                FestivalImg = festival.FestivalImg,
                TicketPriceFormatted = $"€ {festival.TicketPrice:0.00}",
                Reviews = reviews.Select(r => new ReviewViewModel
                {
                    UserName = r.UserName,  // Gebruikersnaam van degene die de review plaatst
                    Content = r.Content,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt  // We zetten CreatedAt nu direct als DateTime in de ViewModel
                }).ToList()
            };

            return View(viewModel);
        }

        // Toon festivals die al voorbij zijn (bijvoorbeeld oude festivals)
        public IActionResult Past()
        {
            var festivals = _festivalService.GetFestivals();
            return View(festivals);
        }

        // Formulier om een nieuw festival toe te voegen
        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "beheerder")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new Festival());
        }

        // Toevoegen aan de database
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

                    festival.FestivalImg = "/images/" + festivalImg.FileName;
                }

                _festivalService.AddFestival(festival); // Gebruik service om festival toe te voegen
                return RedirectToAction("Index");
            }

            return View(festival);
        }



        // Actie om een review toe te voegen
        [HttpPost]
        public IActionResult AddReview(int festivalId, string content, int rating)
        {
            // Controleer of de gebruiker ingelogd is via de sessie
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Als de gebruiker niet ingelogd is, stuur hem naar de loginpagina
                return RedirectToAction("Login", "Account");
            }

            // Maak een nieuwe review aan
            var review = new Review
            {
                FestivalId = festivalId,
                UserId = (int)userId,  // Gebruik de UserId uit de sessie
                Content = content,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            // Voeg de review toe via de ReviewService
            _reviewService.AddReview(review);

            // Terug naar de details van het festival met een succesbericht
            TempData["Success"] = "Je review is succesvol geplaatst!";
            return RedirectToAction("Details", new { id = festivalId });
        }
    }
}