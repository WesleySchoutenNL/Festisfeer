using Festisfeer.Domain.Models;
using System.Collections.Generic;

namespace Festisfeer.Domain.Interfaces
{
    public interface ICommentRepository
    {
        void AddComment(Comment comment);
        List<Comment> GetCommentsByReviewId(int reviewId);

        Comment GetCommentById(int commentId);
        void UpdateComment(Comment comment);
        void DeleteComment(int commentId);
    }
}