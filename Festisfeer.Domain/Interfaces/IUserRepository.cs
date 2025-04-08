using Festisfeer.Domain.Models;

namespace Festisfeer.Domain.Interfaces
{
    public interface IUserRepository
    {
        void RegisterUser(User user);
        User? LoginUser(string email, string password);
        User? GetUserById(int id);
    }
}