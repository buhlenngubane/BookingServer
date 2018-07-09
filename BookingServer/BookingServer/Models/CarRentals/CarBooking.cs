using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.CarRentals
{
    public partial class CarBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        [Required]
        public DateTime BookDate { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; }
        [Required]
        public string PayType { get; set; }
        [Required]
        public bool PayStatus { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Total { get; set; }

        public Car Car { get; set; }
    }
}
