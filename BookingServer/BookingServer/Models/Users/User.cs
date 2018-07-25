using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingServer.Models.Users
{
    public partial class User
    {
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [RegularExpression("[a-z0-9._%+-]+@[a-z0-9.-]+[.][a-z]{2,4}", ErrorMessage = "Incorrect email format.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{7,}", ErrorMessage = "Incorrect password format.")]
        public string Password { get; set; }
        [Required]
        //[RegularExpression("[0-9]{9}", ErrorMessage = "Incorrect phone number format.")]
        public int Phone { get; set; }
        [Required]
        public bool Admin { get; set; }
    }
}
