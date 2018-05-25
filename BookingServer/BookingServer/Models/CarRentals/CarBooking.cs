using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class CarBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int CrentId { get; set; }
        public DateTime BookDate { get; set; }
        public int ReturnDate { get; set; }
        public string PayType { get; set; }
        public int Total { get; set; }

        public CarRental Crent { get; set; }
    }
}
