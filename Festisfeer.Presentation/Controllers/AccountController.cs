using Microsoft.AspNetCore.Mvc;
using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Models;
using Festisfeer.Presentation.Models;

namespace Festisfeer.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Registreren
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    Password = model.Password,
                    Username = model.Username
                };

                _userRepository.RegisterUser(user);
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // Inloggen
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.LoginUser(model.Email, model.Password);
                if (user != null)
                {
                    // Hier kun je een sessie of cookie instellen voor ingelogde gebruikers
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
