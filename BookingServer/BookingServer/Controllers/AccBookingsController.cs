using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Accommodations;
using BookingServer.Models.Users;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers
{
    [Produces("application/json")]
    [Route("api/AccBookings/[action]"), Authorize]
    public class AccBookingsController : Controller
    {
        private readonly AccommodationDBContext _context;
        private readonly UserDBContext _userDB;

        public AccBookingsController(AccommodationDBContext context, UserDBContext userDB)
        {
            _context = context;
            _userDB = userDB;
        }

        // GET: api/Bookings
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<AccBooking> GetAll()
        {
            //if(!id.Equals(1))

            return _context.AccBooking;
        }

        [HttpGet("{AccId}"), Authorize(Policy ="Administrator")]
        public async Task<IActionResult> GetBooking([FromRoute] int AccId)
        {

            if (_context.AccBooking.ToList().Exists(m=>m.AccId.Equals(AccId)))
            {
                var user = _context.AccBooking.Where(m => m.AccId.Equals(AccId));
                Console.WriteLine("Users" + user);
                return Ok(await user.ToListAsync());
            }

            Console.WriteLine("Should be true "+_context.Accommodation.Any(m => m.AccId.Equals(AccId)));

            if (_context.Accommodation.Any(m => m.AccId.Equals(AccId)))
                return NotFound(AccId + ",Not yet booked");
            else
                return BadRequest();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookings([FromRoute] int id)
        {
            if (_context.AccBooking.ToList().Exists(m => m.UserId.Equals(id)))
            {
                var user = _context.AccBooking.Where(m => m.UserId.Equals(id));
                Console.WriteLine("Users" + user);
                return Ok(await user.ToListAsync());
            }
            
                return NotFound("No accommodations booked.");
            //else
                //return BadRequest();
        }

        // PUT: api/Bookings/5
        [HttpPut("{email}")]
        public async Task<IActionResult> Update([FromRoute] string email, [FromBody] AccBooking booking)
        {
            if (!ModelState.IsValid || !UserExists(email))
            {
                return !ModelState.IsValid ? 
                    BadRequest(ModelState) : BadRequest("Email does not exist.");
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

            try
            {
                _context.AccBooking.Add(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return NoContent();
            }

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
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

        private bool UserExists(string email)
        {
            return _userDB.User.Any(e => e.Email.Equals(email));
        }
    }
}