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
using BookingServer.Services.Email;
using BookingServer.Models.Email;

namespace BookingServer.Controllers.CarRentals
{
    [Produces("application/json")]
    [Route("api/CarRentals/CarBookings/[action]"), Authorize]
    public class CarBookingsController : Controller
    {
        private readonly CarRentalDBContext _context;
        private readonly UserDBContext _usersDBContext;
        private readonly IEmailConfiguration _emailConfiguration;

        public CarBookingsController(CarRentalDBContext context, 
            UserDBContext userDBContext, IEmailConfiguration emailConfiguration)
        {
            _context = context;
            _usersDBContext = userDBContext;
            _emailConfiguration = emailConfiguration;
        }

        // GET: api/CarBookings
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<CarBooking> GetCarBooking()
        {
            return _context.CarBooking;
        }

        // GET: api/CarBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(id.ToString()) || User.IsInRole("Admin"))
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

            if (User.Identity.Name.Equals(carBooking.UserId.ToString()) || User.IsInRole("Admin"))
            {
                try
                {
                    var user = await _usersDBContext.User.SingleOrDefaultAsync(s =>
                    s.UserId.Equals(carBooking.UserId));
                    var detail = _context.Car.Where(s => s.CarId.Equals(carBooking.CarId))
                        .Include(s => s.Cmp).Include(s => s.Cmp.Crent).Include(s => s.Ctype);
                    _context.CarBooking.Add(carBooking);
                    await _context.SaveChangesAsync();
                    // EmailAddress address = new EmailAddress();

                    EmailMessage message = new EmailMessage("Flight Booking", "Hi " + user.Name + ",<br/><br/>" +
                        "You have just booked for a car using our a web services, the full details of the booking are: <br/>" +
                        detail.First().Ctype.Name + "<br/>" + detail.First().Cmp.CompanyName + "<br/>" +
                        "<br/>" + "Booked date for pickUp: " + carBooking.BookDate +
                        "<br/>PayType: " + carBooking.PayType + "<br/>Total: R" + carBooking.Total +
                        "<br/><br/>Kind Regards,\nBooking.com");

                    message.FromAddresses.Add(new EmailAddress("Booking.com", "validtest.r.me@gmail.com"));
                    message.ToAddresses.Add(new EmailAddress(user.Name, user.Email));
                    Send send = new Send(message, _emailConfiguration);
                    return CreatedAtAction("GetFlBooking", new { id = carBooking.BookDate }, carBooking);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadRequest("Internal error.");
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