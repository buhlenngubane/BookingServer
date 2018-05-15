using System;
using System.Collections.Generic;

namespace BookingServer.Models.Accommodations
{
    public partial class AccBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int AccId { get; set; }
        public int NumOfNights { get; set; }
        public DateTime BookDate { get; set; }
        public string PayType { get; set; }
        public bool PayStatus { get; set; }
        public int Total { get; set; }

        public Accommodation Acc { get; set; }
    }
}
