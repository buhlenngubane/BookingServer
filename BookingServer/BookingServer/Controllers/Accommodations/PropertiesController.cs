using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Accommodations;
using Microsoft.AspNetCore.Authorization;
using BookingServer.Services;

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

        [HttpGet("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> GetAllProperties([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var accommodation = _context.AccDetail.Where(s => s.Prop.Acc.AccId.Equals(id))
                    .Include(s => s.Prop).Include(s => s.Prop.Acc).Include(s=>s.AccBooking);

                return Ok(await accommodation.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }


        [HttpGet("{country}&{location}")]
        public async Task<IActionResult> GetProperties([FromRoute] string country, string location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @property = _context.Property.Include(s => s.AccDetail).Where(m => m.Acc.Country.Equals(country) && 
                m.Acc.Location.Equals(location));
            

            if (!@property.Any())
            {
                if (location == "")
                {
                    var Country = _context.Property.Include(s => s.AccDetail).Where(m => m.Acc.Country.Equals(country));

                    if (Country.Any())
                    {
                        foreach(Property p in Country)
                        {
                            p.Acc.Property = null;
                        }
                        return Ok(await Country.ToListAsync());
                    }
                }

                return NotFound();
            }

            /*foreach(Property p in @property)
            {
                p.AccDetail = null;
                foreach(AccDetail ac)
            }*/

            return Ok(await @property.ToListAsync());
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
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