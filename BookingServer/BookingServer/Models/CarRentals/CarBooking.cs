using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class CarBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public DateTime BookDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string PayType { get; set; }
        public int Total { get; set; }

        public Car Car { get; set; }
    }
}
