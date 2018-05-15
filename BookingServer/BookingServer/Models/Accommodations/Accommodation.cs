using System;
using System.Collections.Generic;

namespace BookingServer.Models.Accommodations
{
    public partial class Accommodation
    {
        public Accommodation()
        {
            AccBooking = new HashSet<AccBooking>();
            Property = new HashSet<Property>();
        }

        public int AccId { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public byte[] Picture { get; set; }

        public ICollection<AccBooking> AccBooking { get; set; }
        public ICollection<Property> Property { get; set; }
    }
}
