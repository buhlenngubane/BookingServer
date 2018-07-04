using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Accommodations;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers.Accommodations
{
    [Produces("application/json")]
    [Route("api/AccDetails/[action]")]
    public class AccDetailsController : Controller
    {
        private readonly AccommodationDBContext _context;

        public AccDetailsController(AccommodationDBContext context)
        {
            _context = context;
        }

        // GET: api/AccDetails
        [HttpGet]
        public IEnumerable<AccDetail> GetAccDetails()
        {
            return _context.AccDetail;
        }

        // GET: api/AccDetails/5
        [HttpGet("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> GetAccDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var accommodation = _context.AccDetail.Where(s => s.Prop.Acc.AccId.Equals(id))
                    .Include(s=>s.AccBooking)
                    .Include(s=>s.Prop)
                        .ThenInclude(s=>s.Acc);

                if (!accommodation.Any())
                    return Ok("No accommodation for id " + id);

                foreach(AccDetail detail in accommodation)
                {
                    detail.Prop.Picture = null;
                    detail.Prop.Acc.Picture = null;
                    detail.Prop.Acc.Property = null;
                    detail.Prop.AccDetail = null;
                }
                    
                return Ok(await accommodation.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        // PUT: api/AccDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccDetail([FromRoute] int id, [FromBody] AccDetail accDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != accDetail.DetailId)
            {
                return BadRequest();
            }

            _context.Entry(accDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/AccDetails
        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostAccDetail([FromBody] AccDetail accDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AccDetail.Add(accDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccDetail", new { id = accDetail.DetailId }, accDetail);
        }

        // DELETE: api/AccDetails/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteAccDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accDetail = await _context.AccDetail.SingleOrDefaultAsync(m => m.DetailId == id);
            if (accDetail == null)
            {
                return NotFound();
            }

            _context.AccDetail.Remove(accDetail);
            await _context.SaveChangesAsync();

            return Ok(accDetail);
        }

        private bool AccDetailExists(int id)
        {
            return _context.AccDetail.Any(e => e.DetailId == id);
        }
    }
}