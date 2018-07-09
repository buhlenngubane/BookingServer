using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Flights
{
    public partial class FlBooking
    {
        public int BookingId { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int UserId { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int DetailId { get; set; }
        [Required]
        public string FlightType { get; set; }
        [Required]
        public DateTime BookDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Travellers { get; set; }
        [Required]
        public string TravellersNames { get; set; }
        [Required]
        public string TravellersSurnames { get; set; }
        [Required]
        public string PayType { get; set; }
        [Required]
        public bool PayStatus { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Total { get; set; }

        public FlightDetail Detail { get; set; }
    }
}
