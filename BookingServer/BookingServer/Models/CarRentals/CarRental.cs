using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.CarRentals
{
    public partial class CarRental
    {
        public CarRental()
        {
            Ccompany = new HashSet<Ccompany>();
        }

        public int CrentId { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string PhysicalAddress { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int NumOfSuppliers { get; set; }

        public ICollection<Ccompany> Ccompany { get; set; }
    }
}
