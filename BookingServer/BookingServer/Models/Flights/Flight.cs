using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class Flight
    {
        public Flight()
        {
            FlightDetail = new HashSet<FlightDetail>();
        }

        public int FlightId { get; set; }
        public string Locale { get; set; }
        public string Destination { get; set; }
        public int AvFlights { get; set; }

        public ICollection<FlightDetail> FlightDetail { get; set; }
    }
}
