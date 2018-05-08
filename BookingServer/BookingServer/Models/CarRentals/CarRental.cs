using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class CarRental
    {
        public CarRental()
        {
            CarBooking = new HashSet<CarBooking>();
            Company = new HashSet<Company>();
        }

        public int CrentId { get; set; }
        public string Location { get; set; }
        public string PickUp { get; set; }
        public string DropOff { get; set; }

        public ICollection<CarBooking> CarBooking { get; set; }
        public ICollection<Company> Company { get; set; }
    }
}
