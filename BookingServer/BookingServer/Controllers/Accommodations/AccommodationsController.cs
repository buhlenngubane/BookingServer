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

        [HttpGet]
        public async Task<IActionResult> GetAccomPropBook()
        {
            try
            {
                var AccomPropBook = from accomPropBook in _context.Accommodation
                                    join detail in _context.Property.Include(s => s.AccBooking)
                                    on accomPropBook.AccId equals detail.AccId
                                    into Details
                                    //from m in Details.DefaultIfEmpty()
                                    select new { Details };

                return Ok(await AccomPropBook.ToListAsync());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return BadRequest("Noooooooooo!!!!!!");
            }
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
            //if (!locations.Equals(null))
            return Ok();
            //else
                
        }
        

        [HttpGet("{Country}")]
        public async Task<IActionResult> SpecificCountry([FromRoute] string Country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accommodation = from m in _context.Accommodation
                           select m;

            if (!String.IsNullOrEmpty(Country))
            {
                accommodation = accommodation.Where(s => s.Country.Contains(Country));
            }

            if (accommodation == null)
            {
                return NotFound();
            }

            return View(await accommodation.ToListAsync());
        }

        // GET: api/Accommodations/South Africa & Duran
        [HttpGet("{Country}&{Location}")]
        public async Task<IActionResult> SpecificLocation([FromRoute] string Country, string Location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accommodation = from m in _context.Accommodation
                                select m;

            if (!String.IsNullOrEmpty(Country) && !String.IsNullOrEmpty(Country))
            {
                accommodation = 
                    accommodation.Where(s => s.Country.Equals(Country) && s.Location.Contains(Location));
            }

            /*var accommodation = await
                _context.Accommodation.SingleOrDefaultAsync(m => 
                m.Country.Equals(Country) && m.Location.Contains(Location));*/

            if (accommodation == null)
            {
                return NotFound();
            }

            return View(await accommodation.ToListAsync());
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
                _context.Entry(accommodation).State = EntityState.Modified;
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
                    return NoContent();
                }
            }

            return CreatedAtAction("GetAccommodation", 
                new { id = accommodation.AccId }, accommodation);
        }

        // DELETE: api/Accommodations/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteAccommodation([FromRoute] int id)
        {
            if (!ModelState.IsValid || !id.Equals(1))
            {
                return !ModelState.IsValid? 
                    BadRequest(ModelState): BadRequest("Not admin.");
            }

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

        private bool AccommodationExists(int id)
        {
            return _context.Accommodation.Any(e => e.AccId == id);
        }
    }
}