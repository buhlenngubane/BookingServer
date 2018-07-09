using BookingServer.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Services.Email
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
