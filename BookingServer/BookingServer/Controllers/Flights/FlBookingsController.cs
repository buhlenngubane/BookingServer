using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Flights;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers.Flights
{
    [Produces("application/json")]
    [Route("api/Flights/FlBookings/[action]"), Authorize]
    public class FlBookingsController : Controller
    {
        private readonly FlightDBContext _context;

        public FlBookingsController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/FlBookings
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<FlBooking> GetAll()
        {
            return _context.FlBooking;
        }

        [HttpGet("{DetailId}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> GetBooking([FromRoute] int DetailId)
        {

            if (_context.FlBooking.ToList().Exists(m => m.DetailId.Equals(DetailId)))
            {
                var user = _context.FlBooking.Where(m => m.DetailId.Equals(DetailId));
                Console.WriteLine("Users" + user);
                return Ok(await user.ToListAsync());
            }

            if (_context.FlBooking.Any(m => m.DetailId.Equals(DetailId)))
                return NotFound(DetailId + ", not yet booked");
            else
                return BadRequest();
        }

        // GET: api/FlBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(id.ToString()))
            {
                
                if (_context.FlBooking.ToList().Exists(m => m.UserId.Equals(id)))
                {
                    var user = _context.FlBooking.Where(m => m.UserId.Equals(id));
                    Console.WriteLine("Users" + user);
                    return Ok(await user.ToListAsync());
                }

                return NotFound("No flight booked yet.");
            }

            return Unauthorized();
        }

        // PUT: api/FlBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlBooking([FromRoute] int id, [FromBody] FlBooking flBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != flBooking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(flBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlBookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlBookings
        [HttpPost]
        public async Task<IActionResult> New([FromBody] FlBooking flBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(flBooking.UserId.ToString()))
            {
                try
                {
                    _context.FlBooking.Add(flBooking);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetFlBooking", new { id = flBooking.BookDate }, flBooking);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return Json("Internal error");
                }
            }

            return Unauthorized();
        }

        // DELETE: api/FlBookings/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteFlBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flBooking = await _context.FlBooking.SingleOrDefaultAsync(m => m.BookingId == id);
            if (flBooking == null)
            {
                return NotFound();
            }

            _context.FlBooking.Remove(flBooking);
            await _context.SaveChangesAsync();

            return Ok(flBooking);
        }

        private bool FlBookingExists(int id)
        {
            return _context.FlBooking.Any(e => e.BookingId == id);
        }
    }
}