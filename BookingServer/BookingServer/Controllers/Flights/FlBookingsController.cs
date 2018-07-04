using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Flights;
using Microsoft.AspNetCore.Authorization;
using BookingServer.Models.Email;
using BookingServer.Services.Email;
using BookingServer.Models.Users;

namespace BookingServer.Controllers.Flights
{
    [Produces("application/json")]
    [Route("api/Flights/FlBookings/[action]"), Authorize]
    public class FlBookingsController : Controller
    {
        private readonly FlightDBContext _context;
        private readonly UserDBContext _userDB;
        private readonly IEmailConfiguration _emailConfiguration;

        public FlBookingsController(FlightDBContext context,
            UserDBContext userDB, IEmailConfiguration emailConfiguration)
        {
            _context = context;
            _userDB = userDB;
            _emailConfiguration = emailConfiguration;
        }

        // GET: api/FlBookings
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<FlBooking> GetAll()
        {
            return _context.FlBooking;
        }

        // GET: api/FlBookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(id.ToString()) || User.IsInRole("Admin"))
            {
                
                if (_context.FlBooking.ToList().Exists(m => m.UserId.Equals(id)))
                {
                    var user = _context.FlBooking.Where(m => m.UserId.Equals(id)).Include(s=>s.Detail)
                        .ThenInclude(s=>s.Dest)
                        .ThenInclude(s=>s.Flight);
                    Console.WriteLine("Users" + user);
                    return Ok(await user.ToListAsync());
                }

                return NotFound("No flight booked yet.");
            }

            return Unauthorized();
        }

        // PUT: api/FlBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlBooking([FromRoute] int id, [FromBody] FlBooking flBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != flBooking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(flBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlBookingExists(id))
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

        // POST: api/FlBookings
        [HttpPost]
        public async Task<IActionResult> New([FromBody] FlBooking flBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(flBooking.UserId.ToString()) || User.IsInRole("Admin"))
            {
                try
                {
                    var user = await _userDB.User.SingleOrDefaultAsync(s => s.UserId.Equals(flBooking.UserId));
                    var detail = _context.FlightDetail.Where(s => s.DetailId.Equals(flBooking.DetailId))
                        .Include(s => s.Dest).Include(s => s.Dest.Flight);
                    _context.FlBooking.Add(flBooking);
                    await _context.SaveChangesAsync();
                    // EmailAddress address = new EmailAddress();
                    var Return = flBooking.ReturnDate.HasValue ? flBooking.ReturnDate : null;
                    var names = flBooking.TravellersNames.Split(',');
                    var surnames = flBooking.TravellersSurnames.Split(',');
                    string split = user.Name + " " + user.Surname + "<br/>";
                    int index = 0;
                    if (names.Any())
                        foreach(string n in names)
                        {
                            split += n + " " + surnames[index++] + "<br/>";
                        }

                    var str = Return != null ? "ReturnTrip date: " + Return + "<br/>PayType: " + flBooking.PayType : "<br/>PayType: " + flBooking.PayType;

                    EmailMessage message = new EmailMessage("Flight Booking", "Hi " + user.Name + ",<br/><br/>" +
                        "You have just booked for a flight using our a web services, the full details of the booking are: <br/>" +
                        detail.First().Dest.Flight.Locale + "<br/>" + detail.First().Dest.Dest + "<br/>" +
                        flBooking.FlightType + "<br/>" + "Booked date for departure: " + flBooking.BookDate +
                        "<br/>Departure time: " + detail.First().Departure.Split(' ')[1] + "<br/>" +
                         str +
                        "<br/>Number of travellers: " + flBooking.Travellers +
                        "<br/>Travellers names are:<br/>" + split + "<br/>Total: R" + flBooking.Total +
                        "<br/><br/>Kind Regards,<br/>Booking.com");

                    message.FromAddresses.Add(new EmailAddress("Booking.com", "validtest.r.me@gmail.com"));
                    message.ToAddresses.Add(new EmailAddress(user.Name, user.Email));
                    await new Send(message, _emailConfiguration).To(message, _emailConfiguration);
                    return CreatedAtAction("GetFlBooking", new { id = flBooking.BookDate }, flBooking);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadRequest("Internal error");
                }
            }

            return Unauthorized();
        }

        // DELETE: api/FlBookings/5
        [HttpDelete("{id}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> DeleteFlBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flBooking = await _context.FlBooking.SingleOrDefaultAsync(m => m.BookingId == id);
            if (flBooking == null)
            {
                return NotFound();
            }

            _context.FlBooking.Remove(flBooking);
            await _context.SaveChangesAsync();

            return Ok(flBooking);
        }

        private bool FlBookingExists(int id)
        {
            return _context.FlBooking.Any(e => e.BookingId == id);
        }

        
    }
}