using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Accommodations
{
    public partial class AccBooking
    {
        public int BookingId { get; set; }
        
        public int UserId { get; set; }
        
        public int DetailId { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfNights { get; set; }
        [Required]
        public DateTime BookDate { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int RoomsBooked { get; set; }
        [Required]
        public string PayType { get; set; }
        [Required]
        public bool PayStatus { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Total { get; set; }

        public AccDetail Detail { get; set; }
    }
}
