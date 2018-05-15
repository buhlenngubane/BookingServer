using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.CarRentals;

namespace BookingServer.Controllers
{
    [Produces("application/json")]
    [Route("api/CarRentals/[action]")]
    public class CarRentalsController : Controller
    {
        private readonly CarRentalDBContext _context;

        public CarRentalsController(CarRentalDBContext context)
        {
            _context = context;
        }

        // GET: api/CarRentals
        [HttpGet]
        public IEnumerable<CarRental> GetAll()
        {
            return _context.CarRental;
        }

        // GET: api/CarRentals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarRental([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carRental = await _context.CarRental.SingleOrDefaultAsync(m => m.CrentId == id);

            if (carRental == null)
            {
                return NotFound();
            }

            return Ok(carRental);
        }

        // PUT: api/CarRentals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarRental([FromRoute] int id, [FromBody] CarRental carRental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != carRental.CrentId)
            {
                return BadRequest();
            }

            _context.Entry(carRental).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarRentalExists(id))
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

        // POST: api/CarRentals
        [HttpPost]
        public async Task<IActionResult> PostCarRental([FromBody] CarRental carRental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CarRental.Add(carRental);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarRental", new { id = carRental.CrentId }, carRental);
        }

        // DELETE: api/CarRentals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarRental([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carRental = await _context.CarRental.SingleOrDefaultAsync(m => m.CrentId == id);
            if (carRental == null)
            {
                return NotFound();
            }

            _context.CarRental.Remove(carRental);
            await _context.SaveChangesAsync();

            return Ok(carRental);
        }

        private bool CarRentalExists(int id)
        {
            return _context.CarRental.Any(e => e.CrentId == id);
        }
    }
}