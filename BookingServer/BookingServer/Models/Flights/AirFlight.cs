using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class AirFlight
    {
        public AirFlight()
        {
            Detail = new HashSet<Detail>();
        }

        public int FlightId { get; set; }
        public string Locale { get; set; }
        public string Destination { get; set; }
        public int AvFlights { get; set; }
        public string FlightType { get; set; }

        public ICollection<Detail> Detail { get; set; }
    }
}
