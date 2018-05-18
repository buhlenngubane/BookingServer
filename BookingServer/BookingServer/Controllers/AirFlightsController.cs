using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Flights;

namespace BookingServer.Controllers
{
    [Produces("application/json")]
    [Route("api/AirFlights/[action]")]
    public class AirFlightsController : Controller
    {
        private readonly FlightDBContext _context;

        public AirFlightsController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/AirFlights
        [HttpGet]
        public IEnumerable<AirFlight> GetAirFlight()
        {
            return _context.AirFlight;
        }

        // GET: api/AirFlights/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAirFlight([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airFlight = await _context.AirFlight.SingleOrDefaultAsync(m => m.FlightId == id);

            if (airFlight == null)
            {
                return NotFound();
            }

            return Ok(airFlight);
        }

        // PUT: api/AirFlights/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirFlight([FromRoute] int id, [FromBody] AirFlight airFlight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airFlight.FlightId)
            {
                return BadRequest();
            }

            _context.Entry(airFlight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirFlightExists(id))
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

        // POST: api/AirFlights
        [HttpPost]
        public async Task<IActionResult> PostAirFlight([FromBody] AirFlight airFlight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AirFlight.Add(airFlight);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirFlight", new { id = airFlight.FlightId }, airFlight);
        }

        // DELETE: api/AirFlights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirFlight([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airFlight = await _context.AirFlight.SingleOrDefaultAsync(m => m.FlightId == id);
            if (airFlight == null)
            {
                return NotFound();
            }

            _context.AirFlight.Remove(airFlight);
            await _context.SaveChangesAsync();

            return Ok(airFlight);
        }

        private bool AirFlightExists(int id)
        {
            return _context.AirFlight.Any(e => e.FlightId == id);
        }
    }
}