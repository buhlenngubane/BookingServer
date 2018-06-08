using BookingServer.Models.Accommodations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Services
{
    public interface IPropDetail
    {
        bool Stored { get; }

        Task<byte[]> GetAccommodation(int id);
        Task UpdateAccommodatio(int id);
        Task AddAccommodation(IEnumerable<AccDetail> detail);
        Task RemoveAccommodation(AccDetail detail);
    }
}
