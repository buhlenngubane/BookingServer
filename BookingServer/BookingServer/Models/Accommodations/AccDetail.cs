using System;
using System.Collections.Generic;

namespace BookingServer.Models.Accommodations
{
    public partial class AccDetail
    {
        public AccDetail()
        {
            AccBooking = new HashSet<AccBooking>();
        }

        public int DetailId { get; set; }
        public int PropId { get; set; }
        public string PropertyAttr { get; set; }
        public int PricePerNight { get; set; }
        public int AvailableRooms { get; set; }
        public string RoomType { get; set; }
        public DateTime DateAvailableFrom { get; set; }
        public DateTime DateAvailableTo { get; set; }

        public Property Prop { get; set; }
        public ICollection<AccBooking> AccBooking { get; set; }
    }
}
