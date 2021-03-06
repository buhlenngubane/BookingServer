﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Accommodations;
using Microsoft.AspNetCore.Authorization;
using BookingServer.Services;
using Serilog;

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
                    .Include(s => s.Prop).ThenInclude(s => s.Acc).Include(s=>s.AccBooking);

                return Ok(await accommodation.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on properties controllers "+ex);
                Log.Error("Error on properties controllers ", ex);
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
            try
            {
                var @property = _context.Property.Include(s => s.AccDetail).Where(m => m.Acc.Country.Equals(country) &&
                    m.Acc.Location.Equals(location));

                if (!@property.Any())
                {
                    return NotFound();
                }

                return Ok(await @property.ToListAsync());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Log.Error("GetProperty error " + country + " " + location,ex);
                return BadRequest();
            }
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
                Log.Error( string.Format("PutProperty error ", @property), ex);
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
                Log.Error(string.Format("PostProperty error ", @property), ex);
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
                Log.Error(string.Format("PostProperty error ", properties), ex);
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
                return NotFound("No property found.");
            }
            
            try
            {
                _context.Property.Remove(@property);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: "+ex);
                Log.Error("DeleteProperty error " + PropName, ex);
                return new JsonResult("Error deleting!") { StatusCode = 500 };
            }

            return Ok(@property);
        }

        private bool PropertyExists(int id)
        {
            return _context.Property.Any(e => e.PropId == id);
        }
    }
}