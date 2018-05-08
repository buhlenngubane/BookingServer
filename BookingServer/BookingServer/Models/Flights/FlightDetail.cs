using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class FlightDetail
    {
        public int FdetailId { get; set; }
        public int FlightId { get; set; }
        public string FlightStatus { get; set; }
        public int Price { get; set; }
        public string PicDirectory { get; set; }

        public Flight Flight { get; set; }
    }
}
