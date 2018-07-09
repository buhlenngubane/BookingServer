<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class FlightDetail
    {
        public int DetailId { get; set; }
        public int DestId { get; set; }
        public int Cid { get; set; }
        public DateTime Departure { get; set; }
        public DateTime ReturnTrip { get; set; }
        public string Path { get; set; }
        public int Price { get; set; }

        public Company C { get; set; }
        public Destination Dest { get; set; }
    }
}
=======
﻿using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class FlightDetail
    {
        public FlightDetail()
        {
            FlBooking = new HashSet<FlBooking>();
        }

        public int DetailId { get; set; }
        public int DestId { get; set; }
        public int Cid { get; set; }
        public string Departure { get; set; }
        public string ReturnTrip { get; set; }
        public string Path { get; set; }
        public string Price { get; set; }

        public FlCompany C { get; set; }
        public Destination Dest { get; set; }
        public ICollection<FlBooking> FlBooking { get; set; }
    }
}
>>>>>>> UpdateBook
