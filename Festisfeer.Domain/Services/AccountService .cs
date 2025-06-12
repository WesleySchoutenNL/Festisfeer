using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
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
                throw new AccountServiceException("Email, wachtwoord en gebruikersnaam zijn verplicht.");
            }

            try
            {
                _userRepository.RegisterUser(user);
            }
            catch (AccountRepositoryException ex)
            {
                throw new AccountServiceException("Fout bij het registreren van de gebruiker.", ex);
            }
        }

        public User? Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new AccountServiceException("Email en wachtwoord zijn verplicht.");
            }

            try
            {
                var user = _userRepository.LoginUser(email, password);
                return user;
            }
            catch (AccountRepositoryException ex)
            {
                throw new AccountServiceException("Fout bij inloggen van de gebruiker.", ex);
            }
        }

        public User? GetUserById(int id)
        {
            try
            {
                return _userRepository.GetUserById(id);
            }
            catch (AccountRepositoryException ex)
            {
                throw new AccountServiceException("Fout bij ophalen van gebruiker.", ex);
            }
        }
    }
}