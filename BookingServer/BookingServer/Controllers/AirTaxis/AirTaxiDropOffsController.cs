using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.AirTaxis;

namespace BookingServer.Controllers.AirTaxis
{
    [Produces("application/json")]
    [Route("api/AirTaxis/AirTaxiDropOffs/[action]")]
    public class AirTaxiDropOffsController : Controller
    {
        private readonly AirTaxiDBContext _context;

        public AirTaxiDropOffsController(AirTaxiDBContext context)
        {
            _context = context;
        }

        // GET: api/AirTaxiDropOffs
        [HttpGet]
        public IEnumerable<AirTaxiDropOff> GetAirTaxiDropOff()
        {
            return _context.AirTaxiDropOff;
        }

        // GET: api/AirTaxiDropOffs/5
        [HttpGet("{searchString?}")]
        public async Task<IActionResult> Search([FromRoute] string searchString)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                var airTaxiPickUp = _context.AirTaxiDropOff.Where(m => m.DropOff.Contains(searchString));

                return Ok(await airTaxiPickUp.ToListAsync());
            }

            return Ok();
        }

        [HttpGet("{id}&{searchString}")]
        public async Task<IActionResult> SearchPickUp([FromRoute] string searchString, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                var airTaxiPickUp = _context.AirTaxiDropOff.Where(m => m.DropOff.Contains(searchString) && m.PickUpId.Equals(id));

                return Ok(await airTaxiPickUp.ToListAsync());
            }

            return Ok();
        }

        // PUT: api/AirTaxiDropOffs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirTaxiDropOff([FromRoute] int id, [FromBody] AirTaxiDropOff airTaxiDropOff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airTaxiDropOff.DropOffId)
            {
                return BadRequest();
            }

            _context.Entry(airTaxiDropOff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirTaxiDropOffExists(id))
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

        // POST: api/AirTaxiDropOffs
        [HttpPost]
        public async Task<IActionResult> PostAirTaxiDropOff([FromBody] AirTaxiDropOff airTaxiDropOff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AirTaxiDropOff.Add(airTaxiDropOff);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirTaxiDropOff", new { id = airTaxiDropOff.DropOffId }, airTaxiDropOff);
        }

        // DELETE: api/AirTaxiDropOffs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirTaxiDropOff([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airTaxiDropOff = await _context.AirTaxiDropOff.SingleOrDefaultAsync(m => m.DropOffId == id);
            if (airTaxiDropOff == null)
            {
                return NotFound();
            }

            _context.AirTaxiDropOff.Remove(airTaxiDropOff);
            await _context.SaveChangesAsync();

            return Ok(airTaxiDropOff);
        }

        private bool AirTaxiDropOffExists(int id)
        {
            return _context.AirTaxiDropOff.Any(e => e.DropOffId == id);
        }
    }
}