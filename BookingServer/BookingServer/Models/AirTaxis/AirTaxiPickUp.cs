using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxiPickUp
    {
        public AirTaxiPickUp()
        {
            AirTaxiDropOff = new HashSet<AirTaxiDropOff>();
        }

        public int PickUpId { get; set; }
        [Required]
        public string PickUp { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfDrops { get; set; }

        public ICollection<AirTaxiDropOff> AirTaxiDropOff { get; set; }
    }
}
