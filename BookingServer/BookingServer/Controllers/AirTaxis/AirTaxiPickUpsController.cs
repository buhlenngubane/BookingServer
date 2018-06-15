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
    [Route("api/AirTaxis/AirTaxiPickUps/[action]")]
    public class AirTaxiPickUpsController : Controller
    {
        private readonly AirTaxiDBContext _context;

        public AirTaxiPickUpsController(AirTaxiDBContext context)
        {
            _context = context;
        }

        // GET: api/AirTaxiPickUps
        [HttpGet]
        public IEnumerable<AirTaxiPickUp> GetAirTaxiPickUp()
        {
            return _context.AirTaxiPickUp;
        }

        // GET: api/AirTaxiPickUps/5
        [HttpGet("{searchString?}")]
        public async Task<IActionResult> Search([FromRoute] string searchString)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                var airTaxiPickUp = _context.AirTaxiPickUp.Where(m => m.PickUp.Contains(searchString));

                return Ok(await airTaxiPickUp.ToListAsync());
            }

            return Ok();
        }

        // PUT: api/AirTaxiPickUps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirTaxiPickUp([FromRoute] int id, [FromBody] AirTaxiPickUp airTaxiPickUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airTaxiPickUp.PickUpId)
            {
                return BadRequest();
            }

            _context.Entry(airTaxiPickUp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirTaxiPickUpExists(id))
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

        // POST: api/AirTaxiPickUps
        [HttpPost]
        public async Task<IActionResult> PostAirTaxiPickUp([FromBody] AirTaxiPickUp airTaxiPickUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.AirTaxiPickUp.Add(airTaxiPickUp);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAirTaxiPickUp", new { id = airTaxiPickUp.PickUpId }, airTaxiPickUp);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);

                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/AirTaxiPickUps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirTaxiPickUp([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airTaxiPickUp = await _context.AirTaxiPickUp.SingleOrDefaultAsync(m => m.PickUpId == id);
            if (airTaxiPickUp == null)
            {
                return NotFound();
            }

            _context.AirTaxiPickUp.Remove(airTaxiPickUp);
            await _context.SaveChangesAsync();

            return Ok(airTaxiPickUp);
        }

        private bool AirTaxiPickUpExists(int id)
        {
            return _context.AirTaxiPickUp.Any(e => e.PickUpId == id);
        }
    }
}