using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Festisfeer.Presentation.ViewModels;
using System.IO;
using System.Linq;

namespace Festisfeer.Presentation.Controllers
{
    public class FestivalController : Controller
    {
        // Injected services om met data en bestanden te werken
        private readonly FestivalService _festivalService;
        private readonly ReviewService _reviewService;
        private readonly CommentService _commentService;
        private readonly IWebHostEnvironment _hostEnvironment;

        // Constructor: services worden via dependency injection meegegeven
        public FestivalController(
            FestivalService festivalService,
            ReviewService reviewService,
            CommentService commentService,
            IWebHostEnvironment hostEnvironment)
        {
            _festivalService = festivalService;
            _reviewService = reviewService;
            _commentService = commentService;
            _hostEnvironment = hostEnvironment;
        }

        // Methode om alle festivals op te halen en in ViewModels te zetten
        public IActionResult Index()
        {
            // Haal alle festivals op via de service
            var festivals = _festivalService.GetFestivals();

            // Zet elk festival om naar een ViewModel voor presentatie in de view
            var viewModels = festivals.Select(f => new FestivalViewModel
            {
                Id = f.Id,
                Name = f.Name,
                Location = f.Location,
                Period = $"{f.StartDateTime:dd MMM yyyy} - {f.EndDateTime:dd MMM yyyy}", // Format van de data
                Genre = f.Genre,
                FestivalImg = f.FestivalImg,
                TicketPriceFormatted = $"€ {f.TicketPrice:0.00}" // Prijs netjes geformatteerd
            }).ToList();

            // Geef de lijst door aan de view
            return View(viewModels);
        }

        // Details van één specifiek festival, inclusief reviews en reacties
        public IActionResult Details(int id, int? editCommentId = null)
        {
            // Zoek het festival op basis van ID
            var festival = _festivalService.GetFestivalById(id);
            if (festival == null)
                return NotFound(); // Als het niet bestaat, geef 404

            // Haal alle reviews van dit festival op
            var reviews = _reviewService.GetReviewsByFestivalId(id);

            // Maak een ViewModel inclusief de reviews en bijhorende comments
            var viewModel = new FestivalViewModel
            {
                Id = festival.Id,
                Name = festival.Name,
                Location = festival.Location,
                Period = $"{festival.StartDateTime:dd MMM yyyy} - {festival.EndDateTime:dd MMM yyyy}",
                Genre = festival.Genre,
                FestivalImg = festival.FestivalImg,
                TicketPriceFormatted = $"€ {festival.TicketPrice:0.00}",
                Reviews = reviews.Select(rev => new ReviewViewModel //Review Viewmodel vullen
                {
                    Id = rev.Id,
                    UserName = rev.UserName,
                    Content = rev.Content,
                    Rating = rev.Rating,
                    CreatedAt = rev.CreatedAt,
                    Comments = _commentService.GetCommentsByReviewId(rev.Id).Select(com => new CommentViewModel //Comment Viewmodel vullen
                    {
                        Id = com.Id,
                        ReviewId = com.ReviewId,
                        UserId = com.UserId,
                        UserName = com.UserName,
                        Content = com.Content,
                        CreatedAt = com.CreatedAt
                    }).ToList()
                }).ToList()
            };

            // Meegegeven comment-ID doorgeven aan de view, zodat er bewerkt kan worden
            ViewBag.EditCommentId = editCommentId;

            return View(viewModel);
        }

        // Review toevoegen aan een festival
        [HttpPost]
        public IActionResult AddReview(int festivalId, string content, int rating)
        {
            // Kijk of de gebruiker is ingelogd via sessie
            var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //    return RedirectToAction("Login", "Account"); // Niet ingelogd? Doorsturen naar login

            // Maak een nieuwe review aan
            var review = new Review
            {
                FestivalId = festivalId,
                UserId = userId.Value,
                Content = content,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            // Sla review op via de service
            _reviewService.AddReview(review);

            return RedirectToAction("Details", new { id = festivalId });
        }

        // Reactie toevoegen aan een review
        [HttpPost]
        public IActionResult AddComment(int reviewId, int festivalId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //    return RedirectToAction("Login", "Account");

            // Nieuwe comment aanmaken
            var comment = new Comment
            {
                ReviewId = reviewId,
                UserId = userId.Value,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _commentService.AddComment(comment);
            return RedirectToAction("Details", new { id = festivalId });
        }

        // Een bestaande reactie bewerken
        [HttpPost]
        public IActionResult EditComment(int commentId, int festivalId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //    return RedirectToAction("Login", "Account");

            // Zoek de comment en controleer of die van de juiste gebruiker is
            var comment = _commentService.GetCommentById(commentId);
            if (comment == null || comment.UserId != userId)
                return Unauthorized(); // Geen toegang om te bewerken

            // Validatie: lege reactie niet toegestaan
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Details", new { id = festivalId, editCommentId = commentId });
            }

            // Update de comment
            comment.Content = content;
            _commentService.UpdateComment(comment);
            return RedirectToAction("Details", new { id = festivalId });
        }

        // Reactie verwijderen
        [HttpPost]
        public IActionResult DeleteComment(int commentId, int festivalId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var comment = _commentService.GetCommentById(commentId);
            if (comment == null || comment.UserId != userId)
                return Unauthorized();

            // Verwijder de reactie
            _commentService.DeleteComment(commentId);
            return RedirectToAction("Details", new { id = festivalId });
        }

        // Toon festivals uit het verleden (vermoedelijk gefilterd in de view)
        public IActionResult Past()
        {
            var festivals = _festivalService.GetFestivals();
            return View(festivals);
        }

        // Alleen beheerders kunnen festivals toevoegen
        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "beheerder")
                return RedirectToAction("Index", "Home");

            return View(new Festival());
        }

        // Voeg nieuw festival toe inclusief optionele afbeelding
        [HttpPost]
        public IActionResult Add(Festival festival, IFormFile festivalImg)
        {
            // Als model niet geldig is, keer terug naar formulier
            if (!ModelState.IsValid)
                return View(festival);

            if (festivalImg != null)
            {
                // Bepaal opslagpad en kopieer bestand naar wwwroot/images/
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", festivalImg.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    festivalImg.CopyTo(fileStream);
                }

                // Zet bestandsnaam in festival object (zodat het op de site getoond kan worden)
                festival.FestivalImg = "/images/" + festivalImg.FileName;
            }

            // Voeg festival toe via de service
            _festivalService.AddFestival(festival);
            return RedirectToAction("Index");
        }
    }
}