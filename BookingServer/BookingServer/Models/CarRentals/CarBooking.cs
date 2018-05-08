using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class CarBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int CrentId { get; set; }
        public string PayType { get; set; }
        public bool PayStatus { get; set; }
        public int Total { get; set; }

        public CarRental Crent { get; set; }
    }
}
