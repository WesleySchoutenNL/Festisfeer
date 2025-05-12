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
        private readonly FestivalService _festivalService;
        private readonly ReviewService _reviewService;
        private readonly CommentService _commentService;
        private readonly IWebHostEnvironment _hostEnvironment;

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

        public IActionResult Details(int id, int? editCommentId = null)
        {
            var festival = _festivalService.GetFestivalById(id);
            if (festival == null)
                return NotFound();

            var reviews = _reviewService.GetReviewsByFestivalId(id);

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
                    Id = r.Id,
                    UserName = r.UserName,
                    Content = r.Content,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    Comments = _commentService.GetCommentsByReviewId(r.Id).Select(c => new CommentViewModel
                    {
                        Id = c.Id,
                        ReviewId = c.ReviewId,
                        UserId = c.UserId,
                        UserName = c.UserName,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    }).ToList()
                }).ToList()
            };

            ViewBag.EditCommentId = editCommentId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddReview(int festivalId, string content, int rating)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var review = new Review
            {
                FestivalId = festivalId,
                UserId = userId.Value,
                Content = content,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            _reviewService.AddReview(review);
            TempData["Success"] = "Je review is succesvol geplaatst!";
            return RedirectToAction("Details", new { id = festivalId });
        }

        [HttpPost]
        public IActionResult AddComment(int reviewId, int festivalId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var comment = new Comment
            {
                ReviewId = reviewId,
                UserId = userId.Value,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _commentService.AddComment(comment);
            TempData["CommentSuccess"] = "Je reactie is geplaatst!";
            return RedirectToAction("Details", new { id = festivalId });
        }

        [HttpPost]
        public IActionResult EditComment(int commentId, int festivalId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var comment = _commentService.GetCommentById(commentId);
            if (comment == null || comment.UserId != userId)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["CommentError"] = "De reactie mag niet leeg zijn.";
                return RedirectToAction("Details", new { id = festivalId, editCommentId = commentId });
            }

            comment.Content = content;
            _commentService.UpdateComment(comment);

            TempData["CommentSuccess"] = "Je reactie is succesvol bijgewerkt.";
            return RedirectToAction("Details", new { id = festivalId });
        }

        [HttpPost]
        public IActionResult DeleteComment(int commentId, int festivalId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var comment = _commentService.GetCommentById(commentId);
            if (comment == null || comment.UserId != userId)
                return Unauthorized();

            _commentService.DeleteComment(commentId);
            TempData["CommentSuccess"] = "Reactie verwijderd.";
            return RedirectToAction("Details", new { id = festivalId });
        }

        public IActionResult Past()
        {
            var festivals = _festivalService.GetFestivals();
            return View(festivals);
        }

        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "beheerder")
                return RedirectToAction("Index", "Home");

            return View(new Festival());
        }

        [HttpPost]
        public IActionResult Add(Festival festival, IFormFile festivalImg)
        {
            if (!ModelState.IsValid)
                return View(festival);

            if (festivalImg != null)
            {
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", festivalImg.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    festivalImg.CopyTo(fileStream);
                }

                festival.FestivalImg = "/images/" + festivalImg.FileName;
            }

            _festivalService.AddFestival(festival);
            return RedirectToAction("Index");
        }
    }
}