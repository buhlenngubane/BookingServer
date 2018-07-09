using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Models
{
    public class Message
    {
        public string Type { get; set; }
        public string Payload { get; set; }

        public Message(string type, string payload)
        {
            Type = type;
            Payload = payload;
        }
    }
}
