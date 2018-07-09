using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Accommodations;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers.Accommodations
{
    [Produces("application/json")]
    [Route("api/Accommodations/[action]")]
    public class AccommodationsController : Controller
    {
        private readonly AccommodationDBContext _context;

        public AccommodationsController(AccommodationDBContext context)
        {
            _context = context;
        }

        // GET: api/Accommodations
        [HttpGet]
        public IEnumerable<Accommodation> GetAll()
        {
            return _context.Accommodation;
        }

        [HttpGet("{searchString?}")]
        public async Task<IActionResult> Search([FromRoute] string searchString)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Default = new Accommodation();
            Console.WriteLine("Checking if Null");
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                var acc = _context.Accommodation.Where(m => m.Country.Contains(searchString) || 
                m.Location.Contains(searchString));

                return Ok(await acc.ToListAsync());
            }

            return Ok();
        }

        // PUT: api/Accommodations
        [HttpPut, Authorize(Policy ="Administrator")]
        public async Task<IActionResult> PutAccommodation([FromBody] Accommodation accommodation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //_context.Entry(accommodation).State = EntityState.Modified;
                _context.Accommodation.Update(accommodation);
                Console.WriteLine("State change, yet to save.");
                await _context.SaveChangesAsync();
                Console.WriteLine("Saved.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AccommodationExists(accommodation.AccId))
                {
                    return NotFound();
                }
                else
                {
                    Console.WriteLine("Error updating: " + ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/Accommodations/PostAccommodation
        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostAccommodation( 
            [FromBody] Accommodation accommodation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Accommodation.Add(accommodation);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetAccommodation",
                new { id = accommodation.AccId }, accommodation);
            }
            catch (DbUpdateException ex)
            {
                if (AccommodationExists(accommodation.AccId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    Console.WriteLine("Error: "+ex);
                    return BadRequest(ex.Message);
                }
            }

            
        }

        // DELETE: api/Accommodations/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteAccommodation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return  
                    BadRequest(ModelState);
            }

            try
            {
                var accommodation = await
                    _context.Accommodation.SingleOrDefaultAsync(m => m.AccId == id);
                if (accommodation == null)
                {
                    return NotFound();
                }

                _context.Accommodation.Remove(accommodation);
                await _context.SaveChangesAsync();

                return Ok(accommodation);
            }
            catch(Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return BadRequest();
            }
        }

        private bool AccommodationExists(int id)
        {
            return _context.Accommodation.Any(e => e.AccId == id);
        }
    }
}