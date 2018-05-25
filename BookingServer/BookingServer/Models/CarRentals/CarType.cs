using System;
using System.Collections.Generic;

namespace BookingServer.Models.CarRentals
{
    public partial class CarType
    {
        public CarType()
        {
            Car = new HashSet<Car>();
        }

        public int CtypeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int NumOfSeats { get; set; }
        public int NumOfDoors { get; set; }
        public int NumOfAirbags { get; set; }
        public string Transmission { get; set; }
        public byte[] Picture { get; set; }

        public ICollection<Car> Car { get; set; }
    }
}
