using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.CarRentals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using BookingServer.Models.Users;

namespace BookingServer.Controllers.CarRentals
{
    [Produces("application/json")]
    [Route("api/CarRentals/CarBookings/[action]"), Authorize]
    public class CarBookingsController : Controller
    {
        private readonly CarRentalDBContext _context;
        private readonly UserDBContext _usersDBContext;

        public CarBookingsController(CarRentalDBContext context, 
            UserDBContext userDBContext)
        {
            _context = context;
            _usersDBContext = userDBContext;
        }

        // GET: api/CarBookings
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<CarBooking> GetCarBooking()
        {
            return _context.CarBooking;
        }

        [HttpGet("{CarId}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> GetBooking([FromRoute] int CarId)
        {

            if (_context.CarBooking.ToList().Exists(m => m.CarId.Equals(CarId)))
            {
                var user = _context.CarBooking.Where(m => m.CarId.Equals(CarId));
                Console.WriteLine("Users" + user);
                return Ok(await user.ToListAsync());
            }

            //Console.WriteLine("Should be true "+_context.Accommodation.Any(m => m.AccId.Equals(PropId)));

            if (_context.CarBooking.Any(m => m.CarId.Equals(CarId)))
                return NotFound(CarId + ", not yet booked");
            else
                return BadRequest();
        }

        // GET: api/CarBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(id.ToString()))
            {

                if (_context.CarBooking.ToList().Exists(m => m.UserId.Equals(id)))
                {
                    var user = _context.CarBooking.Where(m => m.UserId.Equals(id));
                    Console.WriteLine("Users" + user);
                    return Ok(await user.ToListAsync());
                }

                return NotFound("No car booked yet.");
            }

            return Unauthorized();
        }

        // PUT: api/CarBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarBooking([FromRoute] int id, [FromBody] CarBooking carBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != carBooking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(carBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarBookingExists(id))
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

        // POST: api/CarBookings
        [HttpPost]
        public async Task<IActionResult> New([FromBody] CarBooking carBooking)
        {
            if (!ModelState.IsValid)
            {
                
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(carBooking.UserId.ToString()))
            {
                try
                {
                    _context.CarBooking.Add(carBooking);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetCarBooking", new { id = carBooking.BookingId }, carBooking);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return Json("Internal error.");
                }
            }

            return Unauthorized();
        }

        // DELETE: api/CarBookings/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteCarBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carBooking = await _context.CarBooking.SingleOrDefaultAsync(m => m.BookingId == id);
            if (carBooking == null)
            {
                return NotFound();
            }

            _context.CarBooking.Remove(carBooking);
            await _context.SaveChangesAsync();

            return Ok(carBooking);
        }

        private bool CarBookingExists(int id)
        {
            return _context.CarBooking.Any(e => e.BookingId == id);
        }
    }
}