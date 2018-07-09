using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirBooking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int AirDetailId { get; set; }
        [Required]
        public string TaxiName { get; set; }
        [Required]
        public DateTime BookDate { get; set; }
        public DateTime? ReturnJourney { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Passengers { get; set; }
        [Required]
        public string PayType { get; set; }
        [Required]
        public bool PayStatus { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Total { get; set; }

        public AirDetail AirDetail { get; set; }
    }
}
