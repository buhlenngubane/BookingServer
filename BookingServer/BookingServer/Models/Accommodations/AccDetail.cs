using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Accommodations
{
    public partial class AccDetail
    {
        public AccDetail()
        {
            AccBooking = new HashSet<AccBooking>();
        }

        public int DetailId { get; set; }
        public int PropId { get; set; }
        [Required]
        public string PropertyAttr { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int PricePerNight { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]*", ErrorMessage = "Number must be > 0")]
        public int AvailableRooms { get; set; }
        [Required]
        public string RoomType { get; set; }
        [Required]
        public DateTime DateAvailableFrom { get; set; }
        [Required]
        public DateTime DateAvailableTo { get; set; }

        public Property Prop { get; set; }
        public ICollection<AccBooking> AccBooking { get; set; }
    }
}
