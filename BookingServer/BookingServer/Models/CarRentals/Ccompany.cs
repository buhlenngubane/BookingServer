using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class Ccompany
    {
        public Ccompany()
        {
            Car = new HashSet<Car>();
        }

        public int CmpId { get; set; }
        public int CrentId { get; set; }
        public string CompanyName { get; set; }
        public string FuelPolicy { get; set; }
        public string Mileage { get; set; }
        public int CarCount { get; set; }
        public byte[] Picture { get; set; }

        public CarRental Crent { get; set; }
        public ICollection<Car> Car { get; set; }
    }
}
