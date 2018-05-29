using System;
using System.Collections.Generic;

namespace BookingServer.Models.AirTaxis
{
    public partial class Taxi
    {
        public Taxi()
        {
            AirDetail = new HashSet<AirDetail>();
        }

        public int TaxiId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int NumOfSeats { get; set; }
        public int NumOfBaggage { get; set; }
        public string DriverPolicy { get; set; }

        public ICollection<AirDetail> AirDetail { get; set; }
    }
}
