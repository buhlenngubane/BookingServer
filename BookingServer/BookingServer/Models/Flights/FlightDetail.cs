using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class FlightDetail
    {
        public int DetailId { get; set; }
        public int DestId { get; set; }
        public int Cid { get; set; }
        public DateTime Departure { get; set; }
        public DateTime ReturnTrip { get; set; }
        public string Path { get; set; }
        public int Price { get; set; }

        public FlCompany C { get; set; }
        public Destination Dest { get; set; }
    }
}
