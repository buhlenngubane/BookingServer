using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.AirTaxis;
using Microsoft.AspNetCore.Authorization;

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
        [HttpPut, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PutAirTaxiPickUp([FromBody] AirTaxiPickUp airTaxiPickUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*if (id != airTaxiPickUp.PickUpId)
            {
                return BadRequest();
            }*/

            // _context.Entry(airTaxiPickUp).State = EntityState.Modified;

            try
            {
                _context.AirTaxiPickUp.Update(airTaxiPickUp);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirTaxiPickUpExists(airTaxiPickUp.PickUpId))
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
        [HttpPost, Authorize(Policy = "Administrator")]
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
                Console.WriteLine(ex.Data);

                return BadRequest(ex.Source);
            }
        }

        // DELETE: api/AirTaxiPickUps/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteAirTaxiPickUp([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var airTaxiPickUp = await _context.AirTaxiPickUp
                    .SingleOrDefaultAsync(s=>s.PickUpId.Equals(id))
                    ;

                //var lst = airTaxiPickUp.AirTaxiDropOff.ToList();

                if (airTaxiPickUp == null)
                {
                    return NotFound();
                }

                //airTaxiPickUp.AirTaxiDropOff.Clear();
                
                // _context.AirTaxiDropOff.RemoveRange(airTaxiPickUp.AirTaxiDropOff);
                _context.AirTaxiPickUp.Remove(airTaxiPickUp);
                await _context.SaveChangesAsync();
                return Ok(airTaxiPickUp);
            }
            catch(Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                // Console.WriteLine($"  SaveChanges threw " +
                   // $"{ex.GetType().Name}: {(ex is DbUpdateException ? ex.InnerException.Message : ex.Message)}");
                return BadRequest();
            }

            
        }

        private bool AirTaxiPickUpExists(int id)
        {
            return _context.AirTaxiPickUp.Any(e => e.PickUpId == id);
        }
    }
}