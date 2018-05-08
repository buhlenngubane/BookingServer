using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Flights;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers
{
    [Produces("application/json")]
    [Route("api/FlightDetails/[action]")]
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

            var flightDetail = await _context.FlightDetail.SingleOrDefaultAsync(m => m.FdetailId == id);

            if (flightDetail == null)
            {
                return NotFound();
            }

            return Ok(flightDetail);
        }

        // PUT: api/FlightDetails/5
        [HttpPut("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PutFlightDetail([FromRoute] int id, [FromBody] FlightDetail flightDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != flightDetail.FdetailId)
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
        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostFlightDetail([FromBody] FlightDetail flightDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FlightDetail.Add(flightDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightDetail", new { id = flightDetail.FdetailId }, flightDetail);
        }

        // DELETE: api/FlightDetails/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteFlightDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flightDetail = await _context.FlightDetail.SingleOrDefaultAsync(m => m.FdetailId == id);
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
            return _context.FlightDetail.Any(e => e.FdetailId == id);
        }
    }
}