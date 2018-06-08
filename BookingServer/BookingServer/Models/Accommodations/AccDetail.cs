using System;
using System.Collections.Generic;

namespace BookingServer.Models.Accommodations
{
    public partial class AccDetail
    {
        public int DetailId { get; set; }
        public int PropId { get; set; }
        public int PricePerNight { get; set; }
        public int AvailableRooms { get; set; }
        public DateTime DateAvailable { get; set; }

        public Property Prop { get; set; }
    }
}
