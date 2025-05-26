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
            _festivalRepositoryMock = new Mock<IFestivalRepository>();
            _festivalService = new FestivalService(_festivalRepositoryMock.Object);
        }

        [TestMethod]
        public void GetFestivals_ReturnsCorrectList()
        {
            var festivals = new List<Festival>
            {
                new Festival(1, "Rock Werchter", "Werchter", DateTime.Now, DateTime.Now.AddDays(1), "Rock", 120, "img1.jpg"),
                new Festival(2, "Graspop", "Dessel", DateTime.Now, DateTime.Now.AddDays(2), "Metal", 140, "img2.jpg")
            };

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(festivals);

            var result = _festivalService.GetFestivals();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Rock Werchter", result[0].Name);
            Assert.AreEqual("Graspop", result[1].Name);
        }

        [TestMethod]
        public void GetFestivalById_Test()
        {
            var expectedFestival = new Festival(
                1, "Defqon", "Biddinghuizen", DateTime.Now, DateTime.Now.AddDays(1), "Hardstyle", 150, "defqon.jpg"
            );

            _festivalRepositoryMock.Setup(repo => repo.GetFestivalById(1)).Returns(expectedFestival);

            var result = _festivalService.GetFestivalById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedFestival.Id, result.Id);
        }

        [TestMethod]
        public void AddFestival_StartDateLaterThanEndDateTest()
        {
            var festival = new Festival(0, "Testfest", "Antwerpen", DateTime.Now.AddDays(2), DateTime.Now, "EDM", 100, null);

            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));
            Assert.AreEqual("De einddatum mag niet eerder zijn dan de startdatum.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_NameEmptyTest()
        {
            var festival = new Festival(0, "", "Biddinghuizen", DateTime.Now, DateTime.Now.AddDays(1), "EDM", 50, null);

            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));
            Assert.AreEqual("De naam van het festival mag niet leeg zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_LocationEmptyTest()
        {
            var festival = new Festival(0, "Intents", "", DateTime.Now, DateTime.Now.AddDays(1), "Hardstyle", 50, null);

            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));
            Assert.AreEqual("De locatie van het festival mag niet leeg zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_NegativeTicketPriceTest()
        {
            var festival = new Festival(0, "EDC", "Las Vegas", DateTime.Now, DateTime.Now.AddDays(1), "EDM", -10, null);

            var ex = Assert.ThrowsException<ArgumentException>(() => _festivalService.AddFestival(festival));
            Assert.AreEqual("De ticketprijs mag niet negatief zijn.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_DuplicateFestivalTest()
        {
            var newFestival = new Festival(0, "Tomorrowland", "Boom", new DateTime(2025, 7, 20), new DateTime(2025, 7, 22), "Dance", 200, null);

            var existingFestivals = new List<Festival>
            {
                new Festival(5, "Tomorrowland", "Boom", new DateTime(2025, 7, 20), new DateTime(2025, 7, 22), "Dance", 200, null)
            };

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(existingFestivals);

            var ex = Assert.ThrowsException<InvalidOperationException>(() => _festivalService.AddFestival(newFestival));
            Assert.AreEqual("Een festival met dezelfde naam op die datum bestaat al.", ex.Message);
        }

        [TestMethod]
        public void AddFestival_ValidFestivalTest()
        {
            var festival = new Festival(0, "Defqon", "Brussel", DateTime.Now, DateTime.Now.AddDays(1), "Hardstyle", 100, null);

            _festivalRepositoryMock.Setup(repo => repo.GetFestivals()).Returns(new List<Festival>());

            _festivalService.AddFestival(festival);

            _festivalRepositoryMock.Verify(repo => repo.AddFestival(festival), Times.Once);
        }
    }
}