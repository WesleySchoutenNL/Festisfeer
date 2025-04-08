using Microsoft.AspNetCore.Http;
using Festisfeer.Domain.Interfaces;

namespace Festisfeer.Presentation.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsUserLoggedIn()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") != null;
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Username");
        }

        public int? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
        }
    }
}