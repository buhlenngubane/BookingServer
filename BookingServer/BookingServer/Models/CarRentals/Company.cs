using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class Company
    {
        public int CmpId { get; set; }
        public int CrentId { get; set; }
        public string CompanyName { get; set; }
        public int CarCount { get; set; }
        public string Routine { get; set; }

        public CarRental Crent { get; set; }
    }
}
