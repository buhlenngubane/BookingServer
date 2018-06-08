using BookingServer.Models;
using BookingServer.Models.Accommodations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingServer.Services
{
    public class PropDetail : IPropDetail
    {
        private readonly IDistributedCache _cache;
        private readonly IOptions<Redis> _redis;
        //private readonly AccommodationDBContext _context;
        private bool stored = false;
        private int[] index;
        public bool Stored { get { return stored; } }

        public PropDetail(IDistributedCache cache,
            IOptions<Redis> redis
            //, AccommodationDBContext context
            )
        {
            _cache = cache;
            _redis = redis;
            //_context = context;
        }

        public async Task AddAccommodation(IEnumerable<AccDetail> detail)
        {
            Console.WriteLine("In AddAccommodation");
            foreach(AccDetail d in detail)
            {
                stored = true;
                Console.WriteLine("End " + Stored);
                await _cache.SetStringAsync(d.DetailId.ToString(), 
                     "propId:"+d.PropId + ", pricePerNight:" + d.PricePerNight + ", availableRooms:" + d.AvailableRooms + ", dateAvailable:" + d.DateAvailable, 
                    new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(_redis.Value.ExpiryTime)
                });
                index.Append(d.PropId);
            }
            
            Console.WriteLine("End " + stored);
        }

        public async Task<byte[]> GetAccommodation(int id)
        {
            var Cached = await _cache.GetAsync(id.ToString());

            return Cached;
        }

        public async Task RemoveAccommodation(AccDetail detail)
        {
            await _cache.RemoveAsync(detail.DetailId.ToString());
            //stored = _cache.Get("Detail").IsFixedSize;
            foreach(int i in index)
            {
                if (_cache.Get(i.ToString()).IsFixedSize)
                {
                    stored = true;
                    break;
                }
                else
                    stored = false;
            }
        }

        public IEnumerable<AccDetail> GetProperty(AccDetail[] detail)
        {
            return detail;
        }

        public Task UpdateAccommodatio(int Id)
        {
            
            throw new NotImplementedException();
        }
    }
}
