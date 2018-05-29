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
    [Route("api/AirTaxis/AirDetails/[action]")]
    public class AirDetailsController : Controller
    {
        private readonly AirTaxiDBContext _context;

        public AirDetailsController(AirTaxiDBContext context)
        {
            _context = context;
        }

        // GET: api/AirDetails
        [HttpGet]
        public IEnumerable<AirDetail> GetAirDetail()
        {
            return _context.AirDetail;
        }

        // GET: api/AirDetails/5
        [HttpGet("{search}")]
        public async Task<IActionResult> GetAirDetail([FromRoute] string search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airDetail = _context.AirDetail.Include(s=>s.Taxi).Where(m => m.DropOff.DropOff.Equals(search));

            if (airDetail == null)
            {
                return NotFound();
            }

            return Ok(await airDetail.ToListAsync());
        }

        // PUT: api/AirDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirDetail([FromRoute] int id, [FromBody] AirDetail airDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airDetail.AirDetailId)
            {
                return BadRequest();
            }

            _context.Entry(airDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirDetailExists(id))
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

        // POST: api/AirDetails
        [HttpPost]
        public async Task<IActionResult> PostAirDetail([FromBody] AirDetail airDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AirDetail.Add(airDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAirDetail", new { id = airDetail.AirDetailId }, airDetail);
        }

        // DELETE: api/AirDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airDetail = await _context.AirDetail.SingleOrDefaultAsync(m => m.AirDetailId == id);
            if (airDetail == null)
            {
                return NotFound();
            }

            _context.AirDetail.Remove(airDetail);
            await _context.SaveChangesAsync();

            return Ok(airDetail);
        }

        private bool AirDetailExists(int id)
        {
            return _context.AirDetail.Any(e => e.AirDetailId == id);
        }
    }
}