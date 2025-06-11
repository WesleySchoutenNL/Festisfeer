using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Exceptions;
using static Festisfeer.Domain.Exceptions.AccountExceptions;

namespace Festisfeer.Domain.Services
{
    public class AccountService
    {
        private readonly IUserRepository _userRepository;

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void RegisterUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(user.Username))
            {
                //throw new AccountServiceException("Fout bij toevoegen van review via de service.", ex);

            }

            // Eventuele extra validaties hier toevoegen (bijv. e-mailadres format)
            _userRepository.RegisterUser(user);
        }

        public User? Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                //throw new AccountServiceException("Email en wachtwoord zijn verplicht.");
            }

            var user = _userRepository.LoginUser(email, password);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public User? GetUserById(int id)
        {
            return _userRepository.GetUserById(id);
        }
    }
}