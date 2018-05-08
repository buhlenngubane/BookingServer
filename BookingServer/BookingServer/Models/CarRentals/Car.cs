using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class Car
    {
        public int CarId { get; set; }
        public int CmpId { get; set; }
        public string Name { get; set; }
        public string PicDirectory { get; set; }
    }
}
