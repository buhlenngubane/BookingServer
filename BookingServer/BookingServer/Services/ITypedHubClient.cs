using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Services
{
    public interface ITypedHubClient
    {
        Task BroadcastMessage(string message);
    }
}
