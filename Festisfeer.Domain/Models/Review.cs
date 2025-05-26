using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relatie met festival
        public int FestivalId { get; set; }
        public Festival? Festival { get; set; }

        // Relatie met gebruiker
        public int UserId { get; set; }
        public User User { get; set; } // Gebruiker die de review heeft geplaatst

        // Extra property voor gebruikersnaam
        public string? UserName { get; set; }
    }
}