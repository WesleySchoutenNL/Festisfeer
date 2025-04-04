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
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Genre { get; set; }
        public int TicketPrice { get; set; }
        public string? FestivalImg { get; set; }  // Dit is de URL van de afbeelding
    }
}

