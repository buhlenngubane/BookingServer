using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Flights
{
    public partial class Destination
    {
        public Destination()
        {
            FlightDetail = new HashSet<FlightDetail>();
        }

        public int DestId { get; set; }
        public int FlightId { get; set; }
        [Required]
        public string Dest { get; set; }

        public Flight Flight { get; set; }
        public ICollection<FlightDetail> FlightDetail { get; set; }
    }
}
