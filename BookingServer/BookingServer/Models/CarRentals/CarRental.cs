using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class CarRental
    {
        public CarRental()
        {
            CarBooking = new HashSet<CarBooking>();
            Ccompany = new HashSet<Ccompany>();
        }

        public int CrentId { get; set; }
        public string Location { get; set; }
        public string PhysicalAddress { get; set; }
        public int NumOfSuppliers { get; set; }

        public ICollection<CarBooking> CarBooking { get; set; }
        public ICollection<Ccompany> Ccompany { get; set; }
    }
}
