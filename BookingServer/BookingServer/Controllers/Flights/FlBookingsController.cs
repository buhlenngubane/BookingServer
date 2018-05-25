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

        // GET: api/FlBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlBooking([FromRoute] int id)
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

            return Ok(flBooking);
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
        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostFlBooking([FromBody] FlBooking flBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FlBooking.Add(flBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlBooking", new { id = flBooking.BookDate }, flBooking);
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