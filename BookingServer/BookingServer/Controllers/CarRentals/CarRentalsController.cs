using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.CarRentals;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers.CarRentals
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
        public IEnumerable<CarRental> GetCarRental()
        {
            return _context.CarRental;
        }

        // GET: api/CarRentals/5
        [HttpGet("{searchString?}")]
        public async Task<IActionResult> Search([FromRoute] string searchString)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                var carRental = _context.CarRental.Where(m => m.Location.Contains(searchString));

                return Ok(await carRental.ToListAsync());
            }

            return Ok();
        }

        // PUT: api/CarRentals/5
        [HttpPut, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PutCarRental([FromBody] CarRental carRental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.CarRental.Update(carRental);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarRentalExists(carRental.CrentId))
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
        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostCarRental([FromBody] CarRental carRental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.CarRental.Add(carRental);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCarRental", new { id = carRental.CrentId }, carRental);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);

                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/CarRentals/5
        [HttpDelete("{id}"), Authorize(Policy ="Administrator")]
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
            try
            {

                _context.CarRental.Remove(carRental);
                await _context.SaveChangesAsync();

                return Ok(carRental);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return BadRequest();
            }
        }

        private bool CarRentalExists(int id)
        {
            return _context.CarRental.Any(e => e.CrentId == id);
        }
    }
}