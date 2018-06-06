using System;
using System.Collections.Generic;

namespace BookingServer.Models.Users
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Phone { get; set; }
        public bool Admin { get; set; }
    }
}
