﻿using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BookingServer.Services
{
    public class Booking_Notify:Hub<ITypedHubClient>
    {
        /*public Task Send(string data)
        {
            return Clients.All.SendAsync("Send", data);
        }*/
    }
}
