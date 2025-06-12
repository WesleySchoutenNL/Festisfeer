using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Festisfeer.Domain.Services;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Exceptions;
using System;
using static Festisfeer.Domain.Exceptions.AccountExceptions;

namespace Festisfeer.Testlayer
{
    [TestClass]
    public class AccountServiceTests
    {
        private AccountService _accountService;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _accountService = new AccountService(_userRepositoryMock.Object);
        }

        [TestMethod]
        public void RegisterUser_WithValidUser_CallsRepository()
        {
            // Arrange
            var user = new User(0, "test@mail.com", "password123", "tester", "Visitor");

            // Act
            _accountService.RegisterUser(user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.RegisterUser(user), Times.Once);
        }

        [TestMethod]
        public void RegisterUser_WithMissingFields_ThrowsException()
        {
            // Arrange
            var user = new User(0, "", "", "", "Visitor");

            // Act & Assert
            var ex = Assert.ThrowsException<AccountServiceException>(() => _accountService.RegisterUser(user));
            Assert.AreEqual("Email, wachtwoord en gebruikersnaam zijn verplicht.", ex.Message);
        }

        [TestMethod]
        public void RegisterUser_RepositoryThrowsException_IsWrapped()
        {
            // Arrange
            var user = new User(0, "test@mail.com", "password123", "tester", "Visitor");
            var repoEx = new AccountRepositoryException("DB fout", new Exception());
            _userRepositoryMock.Setup(r => r.RegisterUser(user)).Throws(repoEx);

            // Act
            var ex = Assert.ThrowsException<AccountServiceException>(() => _accountService.RegisterUser(user));

            // Assert
            Assert.AreEqual("Fout bij het registreren van de gebruiker.", ex.Message);
            Assert.AreSame(repoEx, ex.InnerException);
        }

        [TestMethod]
        public void Login_WithValidCredentials_ReturnsUser()
        {
            // Arrange
            var expectedUser = new User(1, "test@mail.com", "password123", "tester", "Visitor");
            _userRepositoryMock.Setup(r => r.LoginUser("test@mail.com", "password123")).Returns(expectedUser);

            // Act
            var result = _accountService.Login("test@mail.com", "password123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUser.Id, result!.Id);
        }

        [TestMethod]
        public void Login_WithEmptyEmailOrPassword_ThrowsException()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.ThrowsException<AccountServiceException>(() => _accountService.Login("", ""));
            Assert.AreEqual("Email en wachtwoord zijn verplicht.", ex.Message);
        }

        [TestMethod]
        public void Login_RepositoryThrowsException_IsWrapped()
        {
            // Arrange
            var repoEx = new AccountRepositoryException("DB fout", new Exception());
            _userRepositoryMock.Setup(r => r.LoginUser("mail", "pass")).Throws(repoEx);

            // Act
            var ex = Assert.ThrowsException<AccountServiceException>(() => _accountService.Login("mail", "pass"));

            // Assert
            Assert.AreEqual("Fout bij inloggen van de gebruiker.", ex.Message);
            Assert.AreSame(repoEx, ex.InnerException);
        }

        [TestMethod]
        public void Login_UserNotFound_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.LoginUser("nope@mail.com", "wrong")).Returns((User?)null);

            // Act
            var result = _accountService.Login("nope@mail.com", "wrong");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetUserById_ValidId_ReturnsUser()
        {
            // Arrange
            var user = new User(1, "email", "password", "user", "Visitor");
            _userRepositoryMock.Setup(r => r.GetUserById(1)).Returns(user);

            // Act
            var result = _accountService.GetUserById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.Id);
        }

        [TestMethod]
        public void GetUserById_RepositoryThrowsException_IsWrapped()
        {
            // Arrange
            var repoEx = new AccountRepositoryException("DB fout", new Exception());
            _userRepositoryMock.Setup(r => r.GetUserById(42)).Throws(repoEx);

            // Act
            var ex = Assert.ThrowsException<AccountServiceException>(() => _accountService.GetUserById(42));

            // Assert
            Assert.AreEqual("Fout bij ophalen van gebruiker.", ex.Message);
            Assert.AreSame(repoEx, ex.InnerException);
        }

        [TestMethod]
        public void GetUserById_UserNotFound_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetUserById(999)).Returns((User?)null);

            // Act
            var result = _accountService.GetUserById(999);

            // Assert
            Assert.IsNull(result);
        }
    }
}