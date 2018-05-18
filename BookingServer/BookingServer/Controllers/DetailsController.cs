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
    [Route("api/Details/[action]")]
    public class DetailsController : Controller
    {
        private readonly FlightDBContext _context;

        public DetailsController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/Details
        [HttpGet]
        public IEnumerable<Detail> GetDetail()
        {
            return _context.Detail;
        }

        // GET: api/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var detail = await _context.Detail.SingleOrDefaultAsync(m => m.FdetailId == id);

            if (detail == null)
            {
                return NotFound();
            }

            return Ok(detail);
        }

        // PUT: api/Details/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetail([FromRoute] int id, [FromBody] Detail detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != detail.FdetailId)
            {
                return BadRequest();
            }

            _context.Entry(detail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetailExists(id))
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

        // POST: api/Details
        [HttpPost]
        public async Task<IActionResult> PostDetail([FromBody] Detail detail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Detail.Add(detail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetail", new { id = detail.FdetailId }, detail);
        }

        // DELETE: api/Details/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var detail = await _context.Detail.SingleOrDefaultAsync(m => m.FdetailId == id);
            if (detail == null)
            {
                return NotFound();
            }

            _context.Detail.Remove(detail);
            await _context.SaveChangesAsync();

            return Ok(detail);
        }

        private bool DetailExists(int id)
        {
            return _context.Detail.Any(e => e.FdetailId == id);
        }
    }
}