using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirDetail
    {
        public AirDetail()
        {
            AirBooking = new HashSet<AirBooking>();
        }

        public int AirDetailId { get; set; }
        public int DropOffId { get; set; }
        public int TaxiId { get; set; }
        [Required]
        public string DriverPolicy { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Price { get; set; }

        public AirTaxiDropOff DropOff { get; set; }
        public Taxi Taxi { get; set; }
        public ICollection<AirBooking> AirBooking { get; set; }
    }
}
