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
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public string Dest { get; set; }

        public Flight Flight { get; set; }
        public ICollection<FlightDetail> FlightDetail { get; set; }
    }
}
