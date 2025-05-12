using Festisfeer.Domain.Interfaces;
using Festisfeer.Domain.Models;
using System.Collections.Generic;

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
            return _commentRepository.GetCommentsByReviewId(reviewId);
        }

        public void AddComment(Comment comment)
        {
            _commentRepository.AddComment(comment);
        }

        public Comment GetCommentById(int commentId)
        {
            return _commentRepository.GetCommentById(commentId);
        }

        public void UpdateComment(Comment comment)
        {
            _commentRepository.UpdateComment(comment);
        }

        public void DeleteComment(int commentId)
        {
            _commentRepository.DeleteComment(commentId);
        }
    }
}