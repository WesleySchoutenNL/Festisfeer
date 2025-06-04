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
        public string? UserName { get; private set; }

        // Constructor voor het aanmaken van een comment
        public Comment(int id, int reviewId, int userId, string? content, DateTime createdAt, string? userName = null)
        {
            Id = id;
            ReviewId = reviewId;
            UserId = userId;
            Content = content;
            CreatedAt = createdAt;
            UserName = userName;
        }

        public void UpdateContent(string newContent)
        {
            Content = newContent;
        }
    }
}
