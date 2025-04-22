namespace Festisfeer.Presentation.ViewModels
{
    public class FestivalViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Period { get; set; }  // Samengevoegde datum
        public string? Genre { get; set; }
        public string? FestivalImg { get; set; }
        public string? TicketPriceFormatted { get; set; }  // Geformatteerde ticketprijs
    }
}