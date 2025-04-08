using System;
using Festisfeer.Domain.Interfaces;
using Microsoft.AspNetCore.Http;


namespace Festisfeer.Domain.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor voor dependency injection
        public UserSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Controleert of de gebruiker is ingelogd op basis van een 'UserId' in de sessie
        public bool IsUserLoggedIn()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") != null;
        }

        // Haalt de gebruikersnaam op uit de sessie
        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Username");
        }

        // Haalt de gebruikers-ID op uit de sessie
        public int? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
        }
    }
}