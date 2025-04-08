using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Models;
using Festisfeer.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserRepository _userRepository;

    public AccountController(UserRepository userRepository)
    {
        _userRepository = userRepository;
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
                // Gebruiker in de sessie opslaan
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens.");
        }
        return View(model);
    }

    // Uitloggen
    public IActionResult Logout()
    {
        // Verwijder gebruiker uit sessie
        HttpContext.Session.Remove("Username");
        HttpContext.Session.Remove("UserId");

        return RedirectToAction("Index", "Home");
    }

    // Register pagina tonen
    [HttpGet]
    public IActionResult Register()
    {
        return View();
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

    // Login pagina tonen
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
}