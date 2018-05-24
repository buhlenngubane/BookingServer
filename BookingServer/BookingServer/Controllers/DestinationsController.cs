using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Flights;

namespace BookingServer.Controllers.Flights
{
    [Produces("application/json")]
    [Route("api/Flights/Destinations/[action]")]
    public class DestinationsController : Controller
    {
        private readonly FlightDBContext _context;

        public DestinationsController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/Destinations
        [HttpGet]
        public IEnumerable<Destination> GetAll()
        {
            return _context.Destination;
        }

        // GET: api/Destinations/5
        [HttpGet("{searchString?}")]
        public async Task<IActionResult> Search([FromRoute] string searchString)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!String.IsNullOrWhiteSpace(searchString))
            {
                var destination = _context.Destination.Where(m => m.Destination1.Contains(searchString));
                return Ok(await destination.ToListAsync());
            }

            return Ok();
        }

        // PUT: api/Destinations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDestination([FromRoute] int id, [FromBody] Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != destination.DestId)
            {
                return BadRequest();
            }

            _context.Entry(destination).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(id))
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

        // POST: api/Destinations
        [HttpPost]
        public async Task<IActionResult> PostDestination([FromBody] Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Destination.Add(destination);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDestination", new { id = destination.DestId }, destination);
        }

        // DELETE: api/Destinations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var destination = await _context.Destination.SingleOrDefaultAsync(m => m.DestId == id);
            if (destination == null)
            {
                return NotFound();
            }

            _context.Destination.Remove(destination);
            await _context.SaveChangesAsync();

            return Ok(destination);
        }

        private bool DestinationExists(int id)
        {
            return _context.Destination.Any(e => e.DestId == id);
        }
    }
}