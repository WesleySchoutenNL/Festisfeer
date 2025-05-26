using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Festisfeer.Domain.Models
{
    //Class voor festivals
    public class Festival
    {
        public int Id { get; private set; }
        public string? Name { get; private set; }
        public string? Location { get; private set; }
        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
        public string? Genre { get; private set; }
        public int TicketPrice { get; private set; }
        public string? FestivalImg { get; private set; }
        
            public Festival() { }


        // Optioneel: constructor om direct een geldig object aan te maken
        public Festival(int id, string? name, string? location, DateTime startDateTime, DateTime endDateTime,
                        string? genre, int ticketPrice, string? festivalImg)
        {
            Id = id;
            Name = name;
            Location = location;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Genre = genre;
            TicketPrice = ticketPrice;
            FestivalImg = festivalImg;
        }

        public void SetFestivalImg(string path)
        {
            FestivalImg = path;
        }
    }
}