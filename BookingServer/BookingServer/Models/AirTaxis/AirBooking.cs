using System;
using System.Collections.Generic;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int PickUpId { get; set; }
        public DateTime BookDate { get; set; }
        public DateTime? ReturnJourney { get; set; }
        public string PayType { get; set; }
        public bool PayStatus { get; set; }
        public int Total { get; set; }

        public AirTaxiPickUp PickUp { get; set; }
    }
}
