using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Flights
{
    public partial class FlCompany
    {
        public FlCompany()
        {
            FlightDetail = new HashSet<FlightDetail>();
        }

        public int Cid { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public byte[] Picture { get; set; }

        public ICollection<FlightDetail> FlightDetail { get; set; }
    }
}
