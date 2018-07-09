using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Models
{
    public class Redis
    {
        public string ConnectionString { get; set; }
        public int ExpiryTime { get; set; }
    }
}
