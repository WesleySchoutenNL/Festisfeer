using System;

namespace Festisfeer.Domain.Interfaces
{
    public interface IUserSessionService
    {
        bool IsUserLoggedIn();
        string GetUsername();
        int? GetUserId();
    }
}