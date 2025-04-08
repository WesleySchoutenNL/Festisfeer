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

        // Register POST
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Email = model.Email,
                        Password = model.Password,
                        Username = model.Username
                    };

                    _userRepository.RegisterUser(user);  // Registreer de gebruiker
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Foutmelding toevoegen aan de ModelState als het e-mailadres of de gebruikersnaam al bestaat
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