using Festisfeer.Domain.Exceptions;
using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using System;
using System.Collections.Generic;
using static Festisfeer.Domain.Exceptions.CommentExceptions;
using static Festisfeer.Domain.Exceptions.ReviewExceptions;

namespace Festisfeer.Domain.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public List<Comment> GetCommentsByReviewId(int reviewId)
        {

            try
            {
                return _commentRepository.GetCommentsByReviewId(reviewId);
            }
            catch (CommentRepositoryException ex)
            {
                throw new CommentServiceException($"Fout bij ophalen van reacties voor review {reviewId}.", ex);
            }
        }

        public void AddComment(Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content))
            {
                throw new InvalidCommentDataException("De comment die je wil invullen is leeg");
            }
            try
            {
                _commentRepository.AddComment(comment);
            }
            catch (CommentRepositoryException ex)
            {
                throw new CommentServiceException("Fout bij toevoegen van reactie.", ex);
            }
        }

        public Comment GetCommentById(int commentId)
        {
            try
            {
                return _commentRepository.GetCommentById(commentId);
            }
            catch (CommentRepositoryException ex)
            {
                throw new CommentServiceException($"Fout bij ophalen van reactie met ID {commentId}.", ex);
            }
        }

        public void UpdateComment(Comment comment)
        {
            try
            {
                _commentRepository.UpdateComment(comment);
            }
            catch (CommentRepositoryException ex)
            {
                throw new CommentServiceException($"Fout bij bijwerken van reactie met ID {comment.Id}.", ex);
            }
        }

        public void DeleteComment(int commentId)
        {
            try
            {
                _commentRepository.DeleteComment(commentId);
            }
            catch (CommentRepositoryException ex)
            {
                throw new CommentServiceException($"Fout bij verwijderen van reactie met ID {commentId}.", ex);
            }
        }
    }
}