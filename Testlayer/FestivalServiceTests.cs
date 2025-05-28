using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Festisfeer.Domain.Services;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using System;
using System.Collections.Generic;
using Festisfeer.Domain.Exceptions;

namespace Festisfeer.Testlayer
{
    [TestClass]
    public class FestivalServiceTests
    {
        private FestivalService _festivalService;
        private Mock<IFestivalRepository> _festivalRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _festivalRepositoryMock = new Mock<IFestivalRepository>();
            _festivalService = new FestivalService(_festivalRepositoryMock.Object);
        }

        [TestMethod]
        public void GetFestivals_ReturnsCorrectList()
        {
            // Arrange – stel verwachte lijst festivals in
            var festivals = new List<Festival>
            {
                new Festival(1, "Rock Werchter", "Werchter", DateTime.Now, DateTime.Now.AddDays(1), "Rock", 120, "img1.jpg"),
                new Festival(2, "Graspop", "Dessel", DateTime.Now, DateTime.Now.AddDays(2), "Metal", 140, "img2.jpg")
            };
            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(festivals);

            // Act – roep de service aan
            var result = _festivalService.GetFestivals();

            // Assert – controleer of de resultaten overeenkomen
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Rock Werchter", result[0].Name);
            Assert.AreEqual("Graspop", result[1].Name);
        }

        [TestMethod]
        public void GetFestivals_ReturnCorrectListFail()
        {
            // Arrange – simuleer exception vanuit repository
            var repoException = new FestivalRepositoryException("Database error", new Exception());
            _festivalRepositoryMock
                .Setup(repo => repo.GetFestivals())
                .Throws(repoException);

            // Act 
            var ex = Assert.ThrowsException<FestivalServiceException>(() => _festivalService.GetFestivals());
            //Assert – controleer of service exception correct wordt doorgegeven
            Assert.IsNotNull(ex);
            Assert.AreEqual("Fout bij ophalen van festivals via de service.", ex.Message);
            Assert.AreSame(repoException, ex.InnerException);
        }

        [TestMethod]
        public void GetFestivalById_TestSucces()
        {
            // Arrange – stel verwacht festival in
            var expectedFestival = new Festival(
                1, "Defqon", "Biddinghuizen", DateTime.Now, DateTime.Now.AddDays(1), "Hardstyle", 150, "defqon.jpg"
            );
            _festivalRepositoryMock.Setup(repo => repo.GetFestivalById(1)).Returns(expectedFestival);

            // Act
            var result = _festivalService.GetFestivalById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFestival.Id, result.Id);
        }

        [TestMethod]
        public void GetFestivalById_Catch()
        {
            // Arrange – simuleer fout bij ophalen via repository
            int festivalId = 42;
            var repoException = new FestivalRepositoryException("Database error", new Exception());
            _festivalRepositoryMock
                .Setup(repo => repo.GetFestivalById(festivalId))
                .Throws(repoException);

            // Act 
            var ex = Assert.ThrowsException<FestivalServiceException>(() => _festivalService.GetFestivalById(festivalId));
            //Assert – controleer of exception correct wordt afgehandeld
            Assert.IsNotNull(ex);
            Assert.AreEqual($"Fout bij ophalen van festival met ID {festivalId} via service.", ex.Message);
            Assert.AreSame(repoException, ex.InnerException);
        }

        [TestMethod]
        public void AddFestival_StartDateLaterThanEndDateTest()
        {
            // Arrange

            var festival = new Festival(0, "Testfest", "Antwerpen", DateTime.Now.AddDays(2), DateTime.Now, "EDM", 100, null);
            //Act
            var ex = Assert.ThrowsException<InvalidFestivalDataException>(() => _festivalService.AddFestival(festival));
            //Assert
            Assert.AreEqual("De einddatum mag niet eerder zijn dan de startdatum.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_NameEmptyTest()
        {
            //Arrange
            var festival = new Festival(0, "", "Biddinghuizen", DateTime.Now, DateTime.Now.AddDays(1), "EDM", 50, null);
            //Act
            var ex = Assert.ThrowsException<InvalidFestivalDataException>(() => _festivalService.AddFestival(festival));
            //Assert
            Assert.AreEqual("De naam van het festival mag niet leeg zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_LocationEmptyTest()
        {
            //Arrange
            var festival = new Festival(0, "Intents", "", DateTime.Now, DateTime.Now.AddDays(1), "Hardstyle", 50, null);
            //Aact
            var ex = Assert.ThrowsException<InvalidFestivalDataException>(() => _festivalService.AddFestival(festival));
            //Assert
            Assert.AreEqual("De locatie van het festival mag niet leeg zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_NegativeTicketPriceTest()
        {
            //Arrange
            var festival = new Festival(0, "EDC", "Las Vegas", DateTime.Now, DateTime.Now.AddDays(1), "EDM", -10, null);
            //Act
            var ex = Assert.ThrowsException<InvalidFestivalDataException>(() => _festivalService.AddFestival(festival));
            //Assert
            Assert.AreEqual("De ticketprijs mag niet negatief zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_DuplicateFestivalTest()
        {
            // Test dat een bestaand festival op dezelfde datum en locatie niet nogmaals mag worden toegevoegd
            //arrange
            var newFestival = new Festival(0, "Tomorrowland", "Boom", new DateTime(2025, 7, 20), new DateTime(2025, 7, 22), "Dance", 200, null);

            var existingFestivals = new List<Festival>
            {
                new Festival(5, "Tomorrowland", "Boom", new DateTime(2025, 7, 20), new DateTime(2025, 7, 22), "Dance", 200, null)
            };

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(existingFestivals);
            //Act
            var ex = Assert.ThrowsException<DuplicateFestivalException>(() => _festivalService.AddFestival(newFestival));
            //Arrange
            Assert.AreEqual("Een festival met dezelfde naam op die datum bestaat al.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_ValidFestivalTest()
        {
            //Arrange
            var festival = new Festival(0, "Defqon", "Brussel", DateTime.Now, DateTime.Now.AddDays(1), "Hardstyle", 100, null);

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(new List<Festival>());
            //Act
            _festivalService.AddFestival(festival);

            // Verify – controleer dat AddFestival precies één keer is aangeroepen
            //Assert
            _festivalRepositoryMock.Verify(repo => repo.AddFestival(festival), Times.Once);
        }
    }
}