using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Models
{
    public class Comment
    {
        public int Id { get; private set; }
        public int ReviewId { get; private set; }
        public int UserId { get; private set; }
        public string? Content { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Alleen voor weergave, mag wel publiek schrijfbaar blijven indien gewenst
        public string? UserName { get; set; }

        // Constructor voor het aanmaken van een comment
        public Comment(int id, int reviewId, int userId, string? content, DateTime createdAt)
        {
            Id = id;
            ReviewId = reviewId;
            UserId = userId;
            Content = content;
            CreatedAt = createdAt;
        }

        public void UpdateContent(string newContent)
        {
            // eventueel validatie hier
            Content = newContent;
        }
    }
}