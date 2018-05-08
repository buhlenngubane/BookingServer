using System;
using System.Collections.Generic;

namespace BookingServer.Models.AirTaxis
{
    public partial class Taxi
    {
        public int TaxiId { get; set; }
        public int AirId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
        public string PicDirectory { get; set; }

        public AirTaxi Air { get; set; }
    }
}
