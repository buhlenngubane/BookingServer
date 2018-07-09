using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.CarRentals
{
    public partial class Car
    {
        public Car()
        {
            CarBooking = new HashSet<CarBooking>();
        }

        public int CarId { get; set; }
        public int CmpId { get; set; }
        public int CtypeId { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int Price { get; set; }

        public Ccompany Cmp { get; set; }
        public CarType Ctype { get; set; }
        public ICollection<CarBooking> CarBooking { get; set; }
    }
}
