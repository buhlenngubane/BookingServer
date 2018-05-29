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
    [Route("api/Accommodations/Properties/[action]")]
    public class PropertiesController : Controller
    {
        private readonly AccommodationDBContext _context;

        public PropertiesController(AccommodationDBContext context)
        {
            _context = context;
        }

        // GET: api/Properties
        [HttpGet]
        public IEnumerable<Property> GetAll()
        {
            return _context.Property;
        }

        // GET: api/Properties/5
        [HttpGet("{accId}")]
        public async Task<IActionResult> GetProperties([FromRoute] int accId)
        {
            var @property =  _context.Property.Where(s => s.AccId.Equals(accId));

            //@property = @property.Where(m => m.AccId.Equals(AccId));
            //await _context.Property.SingleOrDefaultAsync(m => m.AccId.Equals(AccId));

            if (@property == null)
            {
                return NotFound();
            }

            return Ok(await @property.ToListAsync());
        }

        [HttpGet("{AccId}&{PropName}")]
        public async Task<IActionResult> GetProperty([FromRoute] int AccId, string PropName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*var @property = 
                await _context.Property.SingleOrDefaultAsync(m => m.AccId.Equals(AccId) && 
                m.PropName.Contains(PropName));*/
            var @property = from m in _context.Property
                            select m;

            @property = @property.Where(m => m.AccId.Equals(AccId) &&
                m.PropName.Contains(PropName));
            //await _context.Property.SingleOrDefaultAsync(m => m.AccId.Equals(AccId));

            if (@property == null)
            {
                return NotFound();
            }

            return View(await @property.ToListAsync());
        }

        // PUT: api/Properties/5
        [HttpPut, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PutProperty( [FromBody] Property @property)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(@property).State = EntityState.Modified;
                Console.WriteLine("State change, yet to save.");
                await _context.SaveChangesAsync();
                Console.WriteLine("Saved.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PropertyExists(@property.PropId))
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

        // POST: api/Properties
        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostProperty([FromBody] Property @property)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Property.Add(@property);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return NoContent();
            }

            return CreatedAtAction("GetProperty", new { id = @property.PropId }, @property);
        }

        [HttpPost, Authorize(Policy = "Administrator")]
        public async Task<IActionResult> PostProperties([FromBody] Property[] properties)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Property.AddRange(properties);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return NoContent();
            }
            //foreach (Property prop in properties)
            return Ok(properties);//, 
                //new { id = properties..PropId }, properties);
        }

        // DELETE: api/Properties/5
        [HttpDelete("{PropName}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteProperty([FromRoute] string PropName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @property = 
                await _context.Property.SingleOrDefaultAsync(m => m.PropName.Equals(PropName));
            if (@property == null)
            {
                return NotFound();
            }
            
            try
            {
                _context.Property.Remove(@property);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: "+ex);
            }

            return Ok(@property);
        }

        private bool PropertyExists(int id)
        {
            return _context.Property.Any(e => e.PropId == id);
        }
    }
}