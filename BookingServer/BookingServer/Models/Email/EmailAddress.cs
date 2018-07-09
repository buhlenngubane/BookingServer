using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Models.Email
{
    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public EmailAddress(string name, string address)
        {
            Name = name;
            Address = address;
        }
    }
}
