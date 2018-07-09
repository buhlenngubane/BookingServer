using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Models.Email
{
    public class EmailMessage
    {
        public EmailMessage(string subject, string content)
        {
            Subject = subject;
            Content = content;
            ToAddresses = new List<EmailAddress>();
            FromAddresses = new List<EmailAddress>();
        }

        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> FromAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
