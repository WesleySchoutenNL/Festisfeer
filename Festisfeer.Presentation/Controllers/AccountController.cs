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
            //Controleren of alles juist is ingevuld 
            if (ModelState.IsValid)
            {
                var user = _userRepository.LoginUser(model.Email, model.Password);
                if (user != null)
                {
                    //Cookies instellen
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens.");
            }
            return View(model);
        }

        //Register pagina tonen
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Login pagina tonen
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
