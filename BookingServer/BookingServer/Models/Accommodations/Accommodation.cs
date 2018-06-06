using System;
using System.Collections.Generic;

namespace BookingServer.Models.Accommodations
{
    public partial class Accommodation
    {
        public Accommodation()
        {
            Property = new HashSet<Property>();
        }

        public int AccId { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public byte[] Picture { get; set; }

        public ICollection<Property> Property { get; set; }
    }
}
