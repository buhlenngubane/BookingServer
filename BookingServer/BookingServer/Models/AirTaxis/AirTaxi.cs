using System;
using System.Collections.Generic;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxi
    {
        public AirTaxi()
        {
            AirBooking = new HashSet<AirBooking>();
            Taxi = new HashSet<Taxi>();
        }

        public int AirId { get; set; }
        public DateTime PickUp { get; set; }
        public DateTime DropOff { get; set; }
        public int TaxiCount { get; set; }

        public ICollection<AirBooking> AirBooking { get; set; }
        public ICollection<Taxi> Taxi { get; set; }
    }
}
