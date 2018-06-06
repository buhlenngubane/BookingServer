using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.AirTaxis;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers.AirTaxis
{
    [Produces("application/json")]
    [Route("api/AirTaxis/AirBookings/[action]"), Authorize]
    public class AirBookingsController : Controller
    {
        private readonly AirTaxiDBContext _context;

        public AirBookingsController(AirTaxiDBContext context)
        {
            _context = context;
        }

        // GET: api/AirBookings
        [HttpGet, Authorize(Policy ="Administrator")]
        public IEnumerable<AirBooking> GetAirBooking()
        {
            return _context.AirBooking;
        }

        [HttpGet("{AirDetailId}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> GetBooking([FromRoute] int AirDetailId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.AirBooking.ToList().Exists(m => m.AirDetailId.Equals(AirDetailId)))
            {
                var user = _context.AirBooking.Where(m => m.AirDetailId.Equals(AirDetailId));
                Console.WriteLine("Users" + user);
                return Ok(await user.ToListAsync());
            }

            //Console.WriteLine("Should be true "+_context.Accommodation.Any(m => m.AccId.Equals(PropId)));

            if (_context.AirDetail.Any(m => m.AirDetailId.Equals(AirDetailId)))
                return NotFound(AirDetailId + ", not yet booked");
            else
                return BadRequest();
        }

        // GET: api/AirBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAirBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(id.ToString()))
            {

                if (_context.AirBooking.ToList().Exists(m => m.UserId.Equals(id)))
                {
                    var user = _context.AirBooking.Where(m => m.UserId.Equals(id));
                    Console.WriteLine("Users" + user);
                    return Ok(await user.ToListAsync());
                }

                return NotFound("No taxi booked yet.");
            }

            return Unauthorized();
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
        public async Task<IActionResult> New([FromBody] AirBooking airBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(airBooking.UserId.ToString()))
            {
                try
                {
                    _context.AirBooking.Add(airBooking);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetAirBooking", new { id = airBooking.BookingId }, airBooking);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return Json("Internal error.");
                }
            }

            return Unauthorized();
        }

        // DELETE: api/AirBookings/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
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