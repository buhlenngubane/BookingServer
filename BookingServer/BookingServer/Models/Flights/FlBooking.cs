using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class FlBooking
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public DateTime BookDate { get; set; }
        public string PayType { get; set; }
        public bool PayStatus { get; set; }
        public int Total { get; set; }
    }
}
