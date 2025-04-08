﻿using Microsoft.AspNetCore.Mvc;
using Festisfeer.Data.Repositories;
using Festisfeer.Domain.Models;
using Festisfeer.Presentation.Models;
using Microsoft.AspNetCore.Http;

namespace Festisfeer.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

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
                    var user = new User
                    {
                        Email = model.Email,
                        Password = model.Password,
                        Username = model.Username
                    };

                    _userRepository.RegisterUser(user);
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.LoginUser(model.Email, model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Username);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Ongeldige inloggegevens.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}