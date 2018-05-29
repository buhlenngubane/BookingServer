using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class Car
    {
        public int CarId { get; set; }
        public int CmpId { get; set; }
        public int CtypeId { get; set; }
        public int Price { get; set; }

        public Ccompany Cmp { get; set; }
        public CarType Ctype { get; set; }
    }
}
