using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class Detail
    {
        public int FdetailId { get; set; }
        public int FlightId { get; set; }
        public string Company { get; set; }
        public DateTime Departure { get; set; }
        public DateTime ReturnTrip { get; set; }
        public string Path { get; set; }
        public int Price { get; set; }
        public byte[] Picture { get; set; }

        public AirFlight Flight { get; set; }
    }
}
