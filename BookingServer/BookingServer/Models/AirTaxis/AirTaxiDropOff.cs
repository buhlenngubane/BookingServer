using System;
using System.Collections.Generic;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirTaxiDropOff
    {
        public AirTaxiDropOff()
        {
            AirBooking = new HashSet<AirBooking>();
        }

        public int DropOffId { get; set; }
        public int PickUpId { get; set; }
        public string DropOff { get; set; }
        public int TaxiCount { get; set; }

        public AirTaxiPickUp PickUp { get; set; }
        public ICollection<AirBooking> AirBooking { get; set; }
    }
}
