using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.CarRentals
{
    public partial class CarType
    {
        public CarType()
        {
            Car = new HashSet<Car>();
        }

        public int CtypeId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfSeats { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfDoors { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfAirbags { get; set; }
        [Required]
        public string Transmission { get; set; }
        [Required]
        public byte[] Picture { get; set; }

        public ICollection<Car> Car { get; set; }
    }
}
