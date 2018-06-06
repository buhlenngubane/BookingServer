using System;
using System.Collections.Generic;

namespace BookingServer.Models.Accommodations
{
    public partial class Property
    {
        public Property()
        {
            AccBooking = new HashSet<AccBooking>();
        }

        public int PropId { get; set; }
        public int AccId { get; set; }
        public string PropName { get; set; }
        public int PricePerNight { get; set; }
        public int AvailableRooms { get; set; }
        public byte[] Picture { get; set; }

        public Accommodation Acc { get; set; }
        public ICollection<AccBooking> AccBooking { get; set; }
    }
}
