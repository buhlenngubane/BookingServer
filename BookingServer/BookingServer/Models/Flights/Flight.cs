<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

namespace BookingServer.Models.Flights
{
    public partial class Flight
    {
        public Flight()
        {
            Destination = new HashSet<Destination>();
            FlBooking = new HashSet<FlBooking>();
        }

        public int FlightId { get; set; }
        public string Locale { get; set; }
        public int AvFlights { get; set; }

        public ICollection<Destination> Destination { get; set; }
        public ICollection<FlBooking> FlBooking { get; set; }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Flights
{
    public partial class Flight
    {
        public Flight()
        {
            Destination = new HashSet<Destination>();
        }

        public int FlightId { get; set; }
        [Required]
        public string Locale { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int AvFlights { get; set; }

        public ICollection<Destination> Destination { get; set; }
    }
}
>>>>>>> UpdateBook
