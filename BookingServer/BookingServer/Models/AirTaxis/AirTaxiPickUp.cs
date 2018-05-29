using System;
using System.Collections.Generic;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxiPickUp
    {
        public AirTaxiPickUp()
        {
            AirBooking = new HashSet<AirBooking>();
            AirTaxiDropOff = new HashSet<AirTaxiDropOff>();
        }

        public int PickUpId { get; set; }
        public string PickUp { get; set; }
        public int NumOfDrops { get; set; }

        public ICollection<AirBooking> AirBooking { get; set; }
        public ICollection<AirTaxiDropOff> AirTaxiDropOff { get; set; }
    }
}
