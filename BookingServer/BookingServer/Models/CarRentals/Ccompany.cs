using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string FuelPolicy { get; set; }
        [Required]
        public string Mileage { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int CarCount { get; set; }
        [Required]
        public byte[] Picture { get; set; }

        public CarRental Crent { get; set; }
        public ICollection<Car> Car { get; set; }
    }
}
