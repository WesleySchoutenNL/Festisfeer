namespace Festisfeer.Domain.Models
{
    public class Review
    {
        public int Id { get; private set; }
        public string? Content { get; private set; }
        public int Rating { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Relatie met festival
        public int FestivalId { get; private set; }
        public Festival? Festival { get; private set; }

        // Relatie met gebruiker
        public int UserId { get; private set; }
        public User User { get; private set; } // Gebruiker die de review heeft geplaatst

        // Extra property voor gebruikersnaam
        public string? UserName { get; private set; }

        public Review(int id, string content, int rating, DateTime createdAt, int festivalId, int userId, string userName)
        {
            Id = id;
            Content = content;
            Rating = rating;
            CreatedAt = createdAt;
            FestivalId = festivalId;
            UserId = userId;
            UserName = userName;
        }


        // Optioneel: constructor zonder Id (bijvoorbeeld voor nieuwe reviews)
        public Review(string? content, int rating, DateTime createdAt,
                      int festivalId, int userId, string? userName)
        {
            Content = content;
            Rating = rating;
            CreatedAt = createdAt;
            FestivalId = festivalId;
            UserId = userId;
            UserName = userName;
        }
    }
}