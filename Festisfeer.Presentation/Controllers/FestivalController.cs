﻿using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Festisfeer.Presentation.ViewModels;
using System.IO;
using System.Linq;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using System.Xml.Linq;
using Festisfeer.Presentation.InputModels; // Voeg toe voor inputmodel
using Festisfeer.Domain.Exceptions;


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
        public IActionResult AddReview(ReviewInputModel input)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var review = new Review(
                id: 0,
                content: input.Content,
                rating: input.Rating,
                createdAt: DateTime.Now,
                festivalId: input.FestivalId,
                userId: userId.Value,
                userName: null
            );

            _reviewService.AddReview(review);

            return RedirectToAction("Details", new { id = input.FestivalId });
        }

        // Reactie toevoegen aan een review
        [HttpPost]
        public IActionResult AddComment(CommentInputModel input)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var comment = new Comment(
                id: 0,
                reviewId: input.ReviewId,
                userId: userId.Value,
                content: input.Content,
                createdAt: DateTime.Now
            );

            _commentService.AddComment(comment);
            return RedirectToAction("Details", new { id = input.FestivalId });
        }

        // Een bestaande reactie bewerken
        [HttpPost]
        
        public IActionResult EditComment(EditCommentInputModel input)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var comment = _commentService.GetCommentById(input.CommentId);
            if (comment.UserId != userId) return Unauthorized();

            comment.UpdateContent(input.Content);
            _commentService.UpdateComment(comment);

            return RedirectToAction("Details", new { id = input.FestivalId });
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

            return View(new FestivalInputModel());
        }


        // Voeg nieuw festival toe inclusief optionele afbeelding
        [HttpPost]
        public IActionResult Add(FestivalInputModel festivalInput, IFormFile festivalImg)
        {
            if (!ModelState.IsValid)
                return View(festivalInput);

            string? imgPath = null;

            if (festivalImg != null)
            {
                // Opslaan in wwwroot/images/
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                var fileName = Path.GetFileName(festivalImg.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    festivalImg.CopyTo(fileStream);
                }

                imgPath = "/images/" + fileName;
            }

            // Maak het Festival-object volledig aan via constructor (encapsulated)
            var festival = new Festival(
                id: festivalInput.Id,
                name: festivalInput.Name,
                location: festivalInput.Location,
                ticketPrice: festivalInput.TicketPrice,
                startDateTime: festivalInput.StartDateTime,
                endDateTime: festivalInput.EndDateTime,
                genre: festivalInput.Genre,
                festivalImg: imgPath

            );

            // Laat de service validatie en opslag doen
            _festivalService.AddFestival(festival);

            return RedirectToAction("Index");
        }
    }
}