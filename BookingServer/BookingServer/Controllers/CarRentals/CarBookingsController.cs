using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.CarRentals;

namespace BookingServer.Controllers.CarRentals
{
    [Produces("application/json")]
    [Route("api/CarRentals/CarBookings/[action]")]
    public class CarBookingsController : Controller
    {
        private readonly CarRentalDBContext _context;

        public CarBookingsController(CarRentalDBContext context)
        {
            _context = context;
        }

        // GET: api/CarBookings
        [HttpGet]
        public IEnumerable<CarBooking> GetCarBooking()
        {
            return _context.CarBooking;
        }

        // GET: api/CarBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carBooking = await _context.CarBooking.SingleOrDefaultAsync(m => m.BookingId == id);

            if (carBooking == null)
            {
                return NotFound();
            }

            return Ok(carBooking);
        }

        // PUT: api/CarBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarBooking([FromRoute] int id, [FromBody] CarBooking carBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != carBooking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(carBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarBookingExists(id))
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

        // POST: api/CarBookings
        [HttpPost]
        public async Task<IActionResult> PostCarBooking([FromBody] CarBooking carBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CarBooking.Add(carBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarBooking", new { id = carBooking.BookingId }, carBooking);
        }

        // DELETE: api/CarBookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carBooking = await _context.CarBooking.SingleOrDefaultAsync(m => m.BookingId == id);
            if (carBooking == null)
            {
                return NotFound();
            }

            _context.CarBooking.Remove(carBooking);
            await _context.SaveChangesAsync();

            return Ok(carBooking);
        }

        private bool CarBookingExists(int id)
        {
            return _context.CarBooking.Any(e => e.BookingId == id);
        }
    }
}