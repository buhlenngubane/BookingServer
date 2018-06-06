using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class Flight
    {
        public Flight()
        {
            Destination = new HashSet<Destination>();
        }

        public int FlightId { get; set; }
        public string Locale { get; set; }
        public int AvFlights { get; set; }

        public ICollection<Destination> Destination { get; set; }
    }
}
