using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.AirTaxis
{
    public partial class Taxi
    {
        public Taxi()
        {
            AirDetail = new HashSet<AirDetail>();
        }

        public int TaxiId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfSeats { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfBaggage { get; set; }
        [Required]
        public string DriverPolicy { get; set; }

        public ICollection<AirDetail> AirDetail { get; set; }
    }
}
