﻿using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class Company
    {
        public Company()
        {
            FlightDetail = new HashSet<FlightDetail>();
        }

        public int Cid { get; set; }
        public string CompanyName { get; set; }
        public byte[] Picture { get; set; }

        public ICollection<FlightDetail> FlightDetail { get; set; }
    }
}
