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
    [Route("api/Flights/FlightDetails/[action]")]
    public class FlightDetailsController : Controller
    {
        private readonly FlightDBContext _context;

        public FlightDetailsController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/FlightDetails
        [HttpGet]
        public IEnumerable<FlightDetail> GetAll()
        {
            return _context.FlightDetail;
        }

        // GET: api/FlightDetails/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlightDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flightDetail = _context.FlightDetail.Include(c=>c.C).Where(m => m.DestId.Equals(id));

            if (flightDetail == null)
            {
                return NotFound();
            }

            return Ok(await flightDetail.ToListAsync());
        }

        // PUT: api/FlightDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightDetail([FromRoute] int id, [FromBody] FlightDetail flightDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != flightDetail.DetailId)
            {
                return BadRequest();
            }

            _context.Entry(flightDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightDetailExists(id))
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

        // POST: api/FlightDetails
        [HttpPost]
        public async Task<IActionResult> PostFlightDetail([FromBody] FlightDetail flightDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FlightDetail.Add(flightDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightDetail", new { id = flightDetail.DetailId }, flightDetail);
        }

        // DELETE: api/FlightDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlightDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flightDetail = await _context.FlightDetail.SingleOrDefaultAsync(m => m.DetailId == id);
            if (flightDetail == null)
            {
                return NotFound();
            }

            _context.FlightDetail.Remove(flightDetail);
            await _context.SaveChangesAsync();

            return Ok(flightDetail);
        }

        private bool FlightDetailExists(int id)
        {
            return _context.FlightDetail.Any(e => e.DetailId == id);
        }
    }
}