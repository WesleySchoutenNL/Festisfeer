using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;

namespace Festisfeer.Domain.Services
{
    public interface IUserSessionService
    {
        bool IsUserLoggedIn();
        string GetUsername();
        int? GetUserId();
    }
}
