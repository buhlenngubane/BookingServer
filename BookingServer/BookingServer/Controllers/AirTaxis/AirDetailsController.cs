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
        public IEnumerable<AirDetail> GetAll()
        {
            return _context.AirDetail;
        }

        [HttpGet("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> GetDetails([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var airTaxi = _context.AirDetail.Where(s => s.DropOff.PickUp.PickUpId.Equals(id))
                    .Include(s=>s.AirBooking)
                    .Include(s=>s.Taxi)
                    .Include(s=>s.DropOff)
                        .ThenInclude(s=>s.PickUp);

                if (!airTaxi.Any())
                    return Ok("No airTaxi for id " + id);

                foreach(AirDetail detail in airTaxi)
                {
                    detail.DropOff.PickUp.AirTaxiDropOff = null;
                    detail.DropOff.AirDetail = null;
                    detail.Taxi.AirDetail = null;
                }

                return Ok(await airTaxi.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        // GET: api/AirDetails/5
        [HttpGet("{pickUp}&{dropOff}")]
        public async Task<IActionResult> GetAirDetail([FromRoute] string pickUp, string dropOff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var airDetail = _context.AirDetail.Include(s=>s.Taxi)
                .Where(m => m.DropOff.PickUp.PickUp.Equals(pickUp) && m.DropOff.DropOff.Equals(dropOff));

            if (airDetail == null)
            {
                return NotFound();
            }

            return Ok(await airDetail.ToListAsync());
        }

        // PUT: api/AirDetails/5
        [HttpPut("{id}"), Authorize(Policy = "Administrator")]
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
        [HttpPost, Authorize(Policy = "Administrator")]
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
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteAirDetail([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var airDetail = _context.AirDetail.Where(m => m.DropOff.PickUp.PickUpId.Equals(id))
                    .Include(s=>s.AirBooking)
                    .Include(s=>s.DropOff)
                    .ThenInclude(s=>s.PickUp);
                var airPickUps = _context.AirTaxiPickUp.Where(s => s.PickUpId.Equals(id));
                // var join = airPickUps.Join(airDetail,s=>s.PickUpId.Equals(airDetail.))
                if (!airDetail.Any())
                {
                    return NotFound();
                }


                // _context.AirTaxiPickUp.RemoveRange((AirTaxiPickUp)airDetail);
                await _context.SaveChangesAsync();

                return Ok(airDetail);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return BadRequest();
            }
        }

        private bool AirDetailExists(int id)
        {
            return _context.AirDetail.Any(e => e.AirDetailId == id);
        }
    }
}