using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.AirTaxis;

namespace BookingServer.Controllers
{
    [Produces("application/json")]
    [Route("api/AirTaxis/[action]")]
    public class AirTaxisController : Controller
    {
        private readonly AirTaxiDBContext _context;

        public AirTaxisController(AirTaxiDBContext context)
        {
            _context = context;
        }

        // GET: api/AirTaxis
        [HttpGet]
        public IEnumerable<AirTaxi> GetAll()
        {
            return _context.AirTaxi;
        }

        // GET: api/AirTaxis/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAirTaxi([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airTaxi = await _context.AirTaxi.SingleOrDefaultAsync(m => m.AirId == id);

            if (airTaxi == null)
            {
                return NotFound();
            }

            return Ok(airTaxi);
        }

        // PUT: api/AirTaxis/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirTaxi([FromRoute] int id, [FromBody] AirTaxi airTaxi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airTaxi.AirId)
            {
                return BadRequest();
            }

            _context.Entry(airTaxi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirTaxiExists(id))
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

        // POST: api/AirTaxis
        [HttpPost]
        public async Task<IActionResult> PostAirTaxi([FromBody] AirTaxi airTaxi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AirTaxi.Add(airTaxi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirTaxi", new { id = airTaxi.AirId }, airTaxi);
        }

        // DELETE: api/AirTaxis/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirTaxi([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airTaxi = await _context.AirTaxi.SingleOrDefaultAsync(m => m.AirId == id);
            if (airTaxi == null)
            {
                return NotFound();
            }

            _context.AirTaxi.Remove(airTaxi);
            await _context.SaveChangesAsync();

            return Ok(airTaxi);
        }

        private bool AirTaxiExists(int id)
        {
            return _context.AirTaxi.Any(e => e.AirId == id);
        }
    }
}