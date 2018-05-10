using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.AirTaxis;

namespace BookingServer.Controllers
{
    [Produces("application/json")]
    [Route("api/AirBookings")]
    public class AirBookingsController : Controller
    {
        private readonly AirTaxiDBContext _context;

        public AirBookingsController(AirTaxiDBContext context)
        {
            _context = context;
        }

        // GET: api/AirBookings
        [HttpGet]
        public IEnumerable<AirBooking> GetAirBooking()
        {
            return _context.AirBooking;
        }

        // GET: api/AirBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAirBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airBooking = await _context.AirBooking.SingleOrDefaultAsync(m => m.BookingId == id);

            if (airBooking == null)
            {
                return NotFound();
            }

            return Ok(airBooking);
        }

        // PUT: api/AirBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirBooking([FromRoute] int id, [FromBody] AirBooking airBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airBooking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(airBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirBookingExists(id))
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

        // POST: api/AirBookings
        [HttpPost]
        public async Task<IActionResult> PostAirBooking([FromBody] AirBooking airBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AirBooking.Add(airBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirBooking", new { id = airBooking.BookingId }, airBooking);
        }

        // DELETE: api/AirBookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airBooking = await _context.AirBooking.SingleOrDefaultAsync(m => m.BookingId == id);
            if (airBooking == null)
            {
                return NotFound();
            }

            _context.AirBooking.Remove(airBooking);
            await _context.SaveChangesAsync();

            return Ok(airBooking);
        }

        private bool AirBookingExists(int id)
        {
            return _context.AirBooking.Any(e => e.BookingId == id);
        }
    }
}