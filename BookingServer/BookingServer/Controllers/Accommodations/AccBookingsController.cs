using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Accommodations;
using BookingServer.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BookingServer.Services;
using BookingServer.Models;

namespace BookingServer.Controllers.Accommodations
{
    [Produces("application/json")]
    [Route("api/Accommodations/AccBookings/[action]"), Authorize]
    public class AccBookingsController : Controller
    {
        private readonly AccommodationDBContext _context;
        private IHubContext<Booking_Notify, ITypedHubClient> _hubContext;

        public AccBookingsController(AccommodationDBContext context, 
            IHubContext<Booking_Notify, ITypedHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Bookings
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<AccBooking> GetAll()
        {
            //if(!id.Equals(1))

            return _context.AccBooking;
        }

        [HttpGet("{PropId}"), Authorize(Policy ="Administrator")]
        public async Task<IActionResult> GetBooking([FromRoute] int PropId)
        {

            if (_context.AccBooking.ToList().Exists(m=>m.PropId.Equals(PropId)))
            {
                var user = _context.AccBooking.Where(m => m.PropId.Equals(PropId));
                Console.WriteLine("Users" + user);
                return Ok(await user.ToListAsync());
            }

            //Console.WriteLine("Should be true "+_context.Accommodation.Any(m => m.AccId.Equals(PropId)));

            if (_context.Property.Any(m => m.PropId.Equals(PropId)))
                return NotFound(PropId + ", not yet booked");
            else
                return BadRequest();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookings([FromRoute] int id)
        {
            Console.WriteLine(User.IsInRole("Admin"));
            if (User.Identity.Name.Equals(id.ToString()) || User.IsInRole("Admin"))
            {
                if (_context.AccBooking.ToList().Exists(m => m.UserId.Equals(id)))
                {
                    var user = _context.AccBooking.Where(m => m.UserId.Equals(id));
                    Console.WriteLine("Users" + user);
                    return Ok(await user.ToListAsync());
                }

                return NotFound("No accommodation booked yet.");
            }
            return Unauthorized();
        }

        // PUT: api/Bookings/5
        [HttpPut("{email}")]
        public async Task<IActionResult> Update([FromRoute] string email, [FromBody] AccBooking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(booking).State = EntityState.Modified;
                Console.WriteLine("State change, yet to save.");
                await _context.SaveChangesAsync();
                Console.WriteLine("Saved.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookingExists(booking.UserId))
                {
                    return NotFound();
                }
                else
                {
                    Console.WriteLine("Error updating: " + ex);
                }
            }

            return NoContent();
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<IActionResult> New([FromBody] AccBooking booking)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Console.WriteLine("UIYHFufyfugygkif");
            Console.WriteLine(User.Identity.AuthenticationType);

            if (User.Identity.Name.Equals(booking.UserId.ToString()) || User.IsInRole("Administrator"))
                try
                {
                
                _context.AccBooking.Add(booking);
                // await _context.SaveChangesAsync();

                    var detail = _context.AccDetail.SingleOrDefault(s => s.PropId.Equals(booking.PropId));

                    detail.AvailableRooms -= 1;

                    _context.Entry(detail).State = EntityState.Modified;

                    await _context.SaveChangesAsync();

                    await _hubContext.Clients.All.BroadcastMessage("A user has just book for "
                        + _context.Property
                        .Where(m => m.PropId.Equals(booking.PropId))
                        .Select(s => s.PropName) + ", " + detail.AvailableRooms + " left.");

                    return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    //Console.WriteLine(booking.PropId + " total" + booking.Total);
                    return Json("Internal error.");
                }

            

            return Unauthorized();
            
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{UserId}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Delete([FromRoute] int UserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.AccBooking.SingleOrDefaultAsync(m => m.UserId.Equals(UserId));
            if (booking == null)
            {
                return NotFound();
            }

            try
            {
                _context.AccBooking.Remove(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return NoContent();
            }

            return Ok(booking);
        }

        private bool BookingExists(int id)
        {
            return _context.AccBooking.Any(e => e.BookingId == id);
        }
    }
}