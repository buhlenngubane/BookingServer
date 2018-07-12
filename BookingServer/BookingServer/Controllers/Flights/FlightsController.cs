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
    [Route("api/Flights/[action]")]
    public class FlightsController : Controller
    {
        private readonly FlightDBContext _context;

        public FlightsController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/Flights
        [HttpGet]
        public IEnumerable<Flight> GetAll()
        {
            return _context.Flight.Include(s=>s.Destination);
        }

        // GET: api/Flights/5
        [HttpGet("{searchString?}")]
        public async Task<IActionResult> Search([FromRoute] string searchString)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                var flight = _context.Flight.Where(m => m.Locale.Contains(searchString)).Include(m=>m.Destination);



                return Ok(await flight.ToListAsync());
            }

            return Ok();
            
        }

        // PUT: api/Flights/5
        [HttpPut, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PutFlight([FromBody] Flight flight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*if (id != flight.FlightId)
            {
                return BadRequest();
            }*/

            // _context.Entry(flight).State = EntityState.Modified;

            try
            {
                _context.Flight.Update(flight);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(flight.FlightId))
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

        // POST: api/Flights
        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostFlight([FromBody] Flight flight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Flight.Add(flight);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetFlight", new { id = flight.FlightId }, flight);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteFlight([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flight = await _context.Flight.SingleOrDefaultAsync(m => m.FlightId == id);
            if (flight == null)
            {
                return NotFound();
            }

            try { 
                _context.Flight.Remove(flight);
                await _context.SaveChangesAsync();

                return Ok(flight);
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

        private bool FlightExists(int id)
        {
            return _context.Flight.Any(e => e.FlightId == id);
        }
    }
}