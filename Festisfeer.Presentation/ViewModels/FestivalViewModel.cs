namespace Festisfeer.Presentation.ViewModels
{
    public class FestivalViewModel
    {
        // Festival gegevens
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Period { get; set; }  // Samengevoegde datum
        public string? Genre { get; set; }
        public string? FestivalImg { get; set; }
        public string? TicketPriceFormatted { get; set; }  // Geformatteerde ticketprijs

        // Toegevoegde informatie over de reviews
        public List<ReviewViewModel> Reviews { get; set; } // Lijst van reviews
    }

    // Nieuw ViewModel voor de review
    public class ReviewViewModel
    {
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }  
    }
}