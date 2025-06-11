using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Festisfeer.Domain.Services;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using Festisfeer.Domain.Exceptions;
using System;
using System.Collections.Generic;
using static Festisfeer.Domain.Exceptions.CommentExceptions;

namespace Festisfeer.Testlayer
{
    [TestClass]
    public class CommentServiceTests
    {
        private Mock<ICommentRepository> _commentRepositoryMock;
        private CommentService _commentService;

        [TestInitialize]
        public void Setup()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _commentService = new CommentService(_commentRepositoryMock.Object);
        }

        // AddComment
        [TestMethod]
        public void AddComment_ValidComment_CallsRepositoryOnce()
        {
            // Arrange
            var comment = new Comment(0, 1, 1, "Leuke reactie!", DateTime.Now, "User");

            // Act
            _commentService.AddComment(comment);

            // Assert
            _commentRepositoryMock.Verify(r => r.AddComment(comment), Times.Once);
        }

        [TestMethod]
        public void AddComment_EmptyContent_ThrowsInvalidCommentDataException()
        {
            // Arrange
            var comment = new Comment(0, 1, 1, "", DateTime.Now, "User");

            // Act & Assert
            Assert.ThrowsException<InvalidCommentDataException>(() => _commentService.AddComment(comment));
        }

        [TestMethod]
        public void AddComment_RepositoryThrows_ThrowsCommentServiceException()
        {
            // Arrange
            var comment = new Comment(0, 1, 1, "Inhoud", DateTime.Now, "User");
            _commentRepositoryMock
                .Setup(r => r.AddComment(It.IsAny<Comment>()))
                .Throws(new CommentRepositoryException("DB-fout", new Exception()));

            // Act
            void action() => _commentService.AddComment(comment);

            // Assert
            Assert.ThrowsException<CommentServiceException>(action);
        }

        // GetCommentsByReviewId
        [TestMethod]
        public void GetCommentsByReviewId_ValidId_ReturnsList()
        {
            // Arrange
            var reviewId = 1;
            var comments = new List<Comment>
            {
                new Comment(1, reviewId, 2, "Goed!", DateTime.Now, "User1"),
                new Comment(2, reviewId, 3, "Mee eens", DateTime.Now, "User2")
            };
            _commentRepositoryMock.Setup(r => r.GetCommentsByReviewId(reviewId)).Returns(comments);

            // Act
            var result = _commentService.GetCommentsByReviewId(reviewId);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Goed!", result[0].Content);
        }

        [TestMethod]
        public void GetCommentsByReviewId_RepositoryThrows_ThrowsCommentServiceException()
        {
            // Arrange
            var reviewId = 999;
            _commentRepositoryMock
                .Setup(r => r.GetCommentsByReviewId(reviewId))
                .Throws(new CommentRepositoryException("DB-fout", new Exception()));

            // Act
            void action() => _commentService.GetCommentsByReviewId(reviewId);

            // Assert
            Assert.ThrowsException<CommentServiceException>(action);
        }

        // GetCommentById
        [TestMethod]
        public void GetCommentById_ValidId_ReturnsComment()
        {
            // Arrange
            var commentId = 1;
            var expectedComment = new Comment(commentId, 1, 1, "Super!", DateTime.Now, "User");
            _commentRepositoryMock.Setup(r => r.GetCommentById(commentId)).Returns(expectedComment);

            // Act
            var result = _commentService.GetCommentById(commentId);

            // Assert
            Assert.AreEqual("Super!", result.Content);
        }

        [TestMethod]
        public void GetCommentById_RepositoryThrows_ThrowsCommentServiceException()
        {
            // Arrange
            var commentId = 999;
            _commentRepositoryMock
                .Setup(r => r.GetCommentById(commentId))
                .Throws(new CommentRepositoryException("DB-fout", new Exception()));

            // Act
            void action() => _commentService.GetCommentById(commentId);

            // Assert
            Assert.ThrowsException<CommentServiceException>(action);
        }

        // UpdateComment

        [TestMethod]
        public void UpdateComment_ValidComment_CallsRepositoryOnce()
        {
            // Arrange
            var comment = new Comment(1, 1, 1, "Update test", DateTime.Now, "User");

            // Act
            _commentService.UpdateComment(comment);

            // Assert
            _commentRepositoryMock.Verify(r => r.UpdateComment(comment), Times.Once);
        }

        [TestMethod]
        public void UpdateComment_RepositoryThrows_ThrowsCommentServiceException()
        {
            // Arrange
            var comment = new Comment(1, 1, 1, "Update fail", DateTime.Now, "User");
            _commentRepositoryMock
                .Setup(r => r.UpdateComment(It.IsAny<Comment>()))
                .Throws(new CommentRepositoryException("DB-fout", new Exception()));

            // Act
            void action() => _commentService.UpdateComment(comment);

            // Assert
            Assert.ThrowsException<CommentServiceException>(action);
        }

        // DeleteComment
        [TestMethod]
        public void DeleteComment_ValidId_CallsRepositoryOnce()
        {
            // Arrange
            int commentId = 1;

            // Act
            _commentService.DeleteComment(commentId);

            // Assert
            _commentRepositoryMock.Verify(r => r.DeleteComment(commentId), Times.Once);
        }

        [TestMethod]
        public void DeleteComment_RepositoryThrows_ThrowsCommentServiceException()
        {
            // Arrange
            int commentId = 999;
            _commentRepositoryMock
                .Setup(r => r.DeleteComment(commentId))
                .Throws(new CommentRepositoryException("DB-fout", new Exception()));

            // Act
            void action() => _commentService.DeleteComment(commentId);

            // Assert
            Assert.ThrowsException<CommentServiceException>(action);
        }
    }
}