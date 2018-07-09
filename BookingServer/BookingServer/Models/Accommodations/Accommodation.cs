using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Accommodations
{
    public partial class Accommodation
    {
        public Accommodation()
        {
            Property = new HashSet<Property>();
        }

        public int AccId { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public byte[] Picture { get; set; }

        public ICollection<Property> Property { get; set; }
    }
}
