using System;
using System.Collections.Generic;

namespace BookingServer.Models.AirTaxis
{
    public partial class AirDetail
    {
        public AirDetail()
        {
            AirBooking = new HashSet<AirBooking>();
        }

        public int AirDetailId { get; set; }
        public int DropOffId { get; set; }
        public int TaxiId { get; set; }
        public string DriverPolicy { get; set; }
        public int Price { get; set; }

        public AirTaxiDropOff DropOff { get; set; }
        public Taxi Taxi { get; set; }
        public ICollection<AirBooking> AirBooking { get; set; }
    }
}
