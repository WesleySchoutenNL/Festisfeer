using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Festisfeer.Domain.Services;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using System;
using System.Collections.Generic;
using Festisfeer.Domain.Exceptions;
using static Festisfeer.Domain.Exceptions.ReviewExceptions;

namespace Festisfeer.Testlayer
{
    [TestClass]
    public class ReviewServiceTests
    {
        private ReviewService _reviewService;
        private Mock<IReviewRepository> _reviewRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _reviewService = new ReviewService(_reviewRepositoryMock.Object);
        }

        [TestMethod]
        public void AddReview_ValidReview_CallsRepositoryOnce()
        {
            // Arrange
            var review = new Review(
                id: 0,
                content: "Top festival!",
                rating: 5,
                createdAt: DateTime.Now,
                userId: 2,
                festivalId: 1,
                userName: "TestGebruiker"
            );

            // Act
            _reviewService.AddReview(review);

            // Assert
            _reviewRepositoryMock.Verify(r => r.AddReview(review), Times.Once);
        }

        [TestMethod]
        public void AddReview_EmptyContent_ThrowsInvalidReviewDataException()
        {
            // Arrange
            var review = new Review(
                id: 0,
                content: "",
                rating: 4,
                createdAt: DateTime.Now,
                userId: 1,
                festivalId: 1,
                userName: "Test"
            );

            // Act & Assert
            Assert.ThrowsException<InvalidReviewDataException>(() => _reviewService.AddReview(review));
        }

        [TestMethod]
        public void AddReview_NegativeRating_ThrowsInvalidReviewDataException()
        {
            // Arrange
            var review = new Review(
                id: 0,
                content: "Niet zo goed",
                rating: -1,
                createdAt: DateTime.Now,
                userId: 1,
                festivalId: 1,
                userName: "Test"
            );

            // Act & Assert
            Assert.ThrowsException<InvalidReviewDataException>(() => _reviewService.AddReview(review));
        }

        [TestMethod]
        public void AddReview_RepositoryThrows_ThrowsReviewServiceException()
        {
            // Arrange
            var review = new Review(
                id: 0,
                content: "Mooi festival",
                rating: 5,
                createdAt: DateTime.Now,
                userId: 1,
                festivalId: 1,
                userName: "Tester"
            );

            _reviewRepositoryMock
                .Setup(r => r.AddReview(It.IsAny<Review>()))
                .Throws(new ReviewRepositoryException("Databasefout", new Exception()));

            // Act & Assert
            Assert.ThrowsException<ReviewServiceException>(() => _reviewService.AddReview(review));
        }

        [TestMethod]
        public void GetReviewsByFestivalId_ValidId_ReturnsReviewList()
        {
            // Arrange
            var festivalId = 1;
            var expectedReviews = new List<Review>
            {
                new Review(1, "Top", 5, DateTime.Now, 3, festivalId, "User3"),
                new Review(2, "Leuk", 4, DateTime.Now, 4, festivalId, "User4")
            };

            _reviewRepositoryMock
                .Setup(r => r.GetReviewsByFestivalId(festivalId))
                .Returns(expectedReviews);

            // Act
            var result = _reviewService.GetReviewsByFestivalId(festivalId);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Top", result[0].Content);
            Assert.AreEqual("Leuk", result[1].Content);
        }

        [TestMethod]
        public void GetReviewsByFestivalId_RepositoryThrows_ThrowsReviewServiceException()
        {
            // Arrange
            var festivalId = 1;

            _reviewRepositoryMock
                .Setup(r => r.GetReviewsByFestivalId(festivalId))
                .Throws(new ReviewRepositoryException("DB error", new Exception()));

            // Act & Assert
            Assert.ThrowsException<ReviewServiceException>(() => _reviewService.GetReviewsByFestivalId(festivalId));
        }
    }
}