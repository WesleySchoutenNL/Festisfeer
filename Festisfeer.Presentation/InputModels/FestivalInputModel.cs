using System;
using Microsoft.AspNetCore.Http;

namespace Festisfeer.Presentation.InputModels
{
    public class FestivalInputModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Genre { get; set; }
        public int TicketPrice { get; set; }
        public IFormFile? FestivalImg { get; set; }
    }
}