using Microsoft.AspNetCore.Mvc;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Festisfeer.Presentation.Models;
using Microsoft.AspNetCore.Http;

namespace Festisfeer.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;  // Gebruik de interface

        // Constructor maakt gebruik van de interface
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Register GET
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User(
                        id: 0, // ID wordt toegekend door de database
                        email: model.Email,
                        password: model.Password,
                        username: model.Username,
                        role: "Visitor" // standaardrol
                    );

                    _userRepository.RegisterUser(user);
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        // Login GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login POST
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.LoginUser(model.Email, model.Password);  // Log in gebruiker
                if (user != null)
                {
                    // Sla de gebruiker ID, Username en Role op in de sessie
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);  // Rol wordt nu opgeslagen in de sessie

                    return RedirectToAction("Index", "Home");
                }

                // Voeg foutmelding toe als inloggen mislukt
                ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens.");
            }

            return View(model);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  // Verwijder de sessie
            return RedirectToAction("Index", "Home");
        }
    }
}