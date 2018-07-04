using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxiDropOff
    {
        public AirTaxiDropOff()
        {
            AirDetail = new HashSet<AirDetail>();
        }

        public int DropOffId { get; set; }
        public int PickUpId { get; set; }
        [Required]
        public string DropOff { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int TaxiCount { get; set; }

        public AirTaxiPickUp PickUp { get; set; }
        public ICollection<AirDetail> AirDetail { get; set; }
    }
}
