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
        [Required, RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$", 
            ErrorMessage ="Email pattern incorrect")]
        public string Email { get; set; }
        [Required, MinLength(8), 
            RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}", 
            ErrorMessage ="Password pattern incorrect")]
        public string Password { get; set; }
        [Required, RegularExpression(@"\d{10}", 
            ErrorMessage ="Phone number must have 10 digits")]
        public string Phone { get; set; }
    }
}
