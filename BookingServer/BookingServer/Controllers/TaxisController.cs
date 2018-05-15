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
    [Route("api/Taxis/[action]")]
    public class TaxisController : Controller
    {
        private readonly AirTaxiDBContext _context;

        public TaxisController(AirTaxiDBContext context)
        {
            _context = context;
        }

        // GET: api/Taxis
        [HttpGet]
        public IEnumerable<Taxi> GetAll()
        {
            return _context.Taxi;
        }

        // GET: api/Taxis/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxi([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxi = await _context.Taxi.SingleOrDefaultAsync(m => m.TaxiId == id);

            if (taxi == null)
            {
                return NotFound();
            }

            return Ok(taxi);
        }

        // PUT: api/Taxis/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaxi([FromRoute] int id, [FromBody] Taxi taxi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taxi.TaxiId)
            {
                return BadRequest();
            }

            _context.Entry(taxi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxiExists(id))
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

        // POST: api/Taxis
        [HttpPost]
        public async Task<IActionResult> PostTaxi([FromBody] Taxi taxi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Taxi.Add(taxi);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaxi", new { id = taxi.TaxiId }, taxi);
        }

        // DELETE: api/Taxis/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxi([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taxi = await _context.Taxi.SingleOrDefaultAsync(m => m.TaxiId == id);
            if (taxi == null)
            {
                return NotFound();
            }

            _context.Taxi.Remove(taxi);
            await _context.SaveChangesAsync();

            return Ok(taxi);
        }

        private bool TaxiExists(int id)
        {
            return _context.Taxi.Any(e => e.TaxiId == id);
        }
    }
}