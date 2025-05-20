using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Festisfeer.Domain.Services;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            // Arrange: Maak een mock van de repository
            _festivalRepositoryMock = new Mock<IFestivalRepository>();
            _festivalService = new FestivalService(_festivalRepositoryMock.Object);
        }

        [TestMethod]
        public void GetFestivals_ReturnsCorrectList()
        {
            // Arrange
            var festivals = new List<Festival>
            {
                new Festival { Id = 1, Name = "Rock Werchter" },
                new Festival { Id = 2, Name = "Graspop" }
            };

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(festivals);

            // Act
            var result = _festivalService.GetFestivals();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Rock Werchter", result[0].Name);
            Assert.AreEqual("Graspop", result[1].Name);
        }

        [TestMethod]
        public void GetFestivalById_Test()
        {
            // Arrange
            var expectedFestival = new Festival
            {
                Id = 1,
                Name = "Defqon",
                Location = "Biddinghuizen",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddDays(1),
                Genre = "Hardstyle",
                TicketPrice = 150,
                FestivalImg = "defqon.jpg"
            };

            _festivalRepositoryMock.Setup(repo => repo.GetFestivalById(1)).Returns(expectedFestival);
            //Act
            var result = _festivalService.GetFestivalById(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFestival.Id, result.Id);
            //Assert.AreEqual(expectedFestival.Name, result.Name);
            //Assert.AreEqual(expectedFestival.Location, result.Location);

        }


        [TestMethod]
        public void AddFestival_StartDateLaterThanEndDateTest()
        {
            // Arrange
            var festival = new Festival
            {
                Name = "Testfest",
                Location = "Antwerpen",
                StartDateTime = DateTime.Now.AddDays(2),
                EndDateTime = DateTime.Now,
                TicketPrice = 100
            };

            // Act
            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));

            // Assert
            Assert.AreEqual("De einddatum mag niet eerder zijn dan de startdatum.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_NameEmptyTest()
        {
            // Arrange
            var festival = new Festival
            {
                Name = "",
                Location = "Biddingshuizen",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddDays(1),
                TicketPrice = 50
            };

            // Act
            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));

            // Assert
            Assert.AreEqual("De naam van het festival mag niet leeg zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_LocationEmptyTest()
        {
            // Arrange
            var festival = new Festival
            {
                Name = "Intents",
                Location = "",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddDays(1),
                TicketPrice = 50
            };

            // Act
            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));

            // Assert
            Assert.AreEqual("De locatie van het festival mag niet leeg zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_NegativeTicketPriceTest()
        {
            // Arrange
            var festival = new Festival
            {
                Name = "EDC",
                Location = "Las Vegas",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddDays(1),
                TicketPrice = -10
            };

            // Act
            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));

            // Assert
            Assert.AreEqual("De ticketprijs mag niet negatief zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_DuplicateFestivalTest()
        {
            // Arrange
            var newFestival = new Festival
            {
                Name = "Tomorrowland",
                StartDateTime = new DateTime(2025, 7, 20),
                Location = "Boom",
                EndDateTime = new DateTime(2025, 7, 22),
                TicketPrice = 200
            };

            var existingFestivals = new List<Festival>
            {
                new Festival { Name = "Tomorrowland", StartDateTime = new DateTime(2025, 7, 20) }
            };

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(existingFestivals);

            // Act
            var ex = Assert.ThrowsException<InvalidOperationException>(() => _festivalService.AddFestival(newFestival));

            // Assert
            Assert.AreEqual("Een festival met dezelfde naam op die datum bestaat al.", ex.Message);
        }



        [TestMethod]
        public void AddFestival_ValidFestivalTest()
        {
            // Arrange
            var festival = new Festival
            {
                Name = "Defqon",
                Location = "Brussel",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddDays(1),
                TicketPrice = 100
            };

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(new List<Festival>()); // geen duplicaten

            // Act
            _festivalService.AddFestival(festival);

            // Assert: controleer of AddFestival op de repository werd aangeroepen met het juiste festival
            _festivalRepositoryMock.Verify(repo => repo.AddFestival(festival), Times.Once);
        }
    }
}