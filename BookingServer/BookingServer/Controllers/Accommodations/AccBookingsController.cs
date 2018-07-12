using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Accommodations;
using BookingServer.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BookingServer.Services;
using BookingServer.Models.Email;
using BookingServer.Services.Email;

namespace BookingServer.Controllers.Accommodations
{
    [Produces("application/json")]
    [Route("api/Accommodations/AccBookings/[action]"), Authorize]
    public class AccBookingsController : Controller
    {
        private readonly AccommodationDBContext _context;
        private readonly UserDBContext _userDB;
        private readonly IEmailConfiguration _emailConfiguration;
        private IHubContext<Booking_Notify, ITypedHubClient> _hubContext;

        public AccBookingsController(AccommodationDBContext context,
            UserDBContext userDB, IEmailConfiguration emailConfiguration,
            IHubContext<Booking_Notify, ITypedHubClient> hubContext)
        {
            _context = context;
            _userDB = userDB;
            _emailConfiguration = emailConfiguration;
            _hubContext = hubContext;
        }

        // GET: api/Bookings
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<AccBooking> GetAll()
        {
            return _context.AccBooking;
        }

        [HttpGet("{PropId}"), Authorize(Policy ="Administrator")]
        public async Task<IActionResult> GetBooking([FromRoute] int PropId)
        {

            if (_context.AccBooking.ToList().Exists(m=>m.Detail.PropId.Equals(PropId)))
            {
                var user = _context.AccBooking.Where(m => m.Detail.PropId.Equals(PropId));
                Console.WriteLine("Users" + user);
                return Ok(await user.ToListAsync());
            }

            //Console.WriteLine("Should be true "+_context.Accommodation.Any(m => m.AccId.Equals(PropId)));

            if (_context.Property.Any(m => m.PropId.Equals(PropId)))
                return NotFound(PropId + ", not yet booked");
            else
                return BadRequest();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookings([FromRoute] int id)
        {
            Console.WriteLine(User.IsInRole("Admin"));
            if (User.Identity.Name.Equals(id.ToString()) || User.IsInRole("Admin"))
            {
                if (_context.AccBooking.ToList().Exists(m => m.UserId.Equals(id)))
                {
                    var user = _context.AccBooking.Where(m => m.UserId.Equals(id)).Include(s=>s.Detail)
                        .ThenInclude(s=>s.Prop).ThenInclude(s=>s.Acc);
                    Console.WriteLine("Users" + user);
                    return Ok(await user.ToListAsync());
                }

                return NotFound("No accommodation booked yet.");
            }
            return Unauthorized();
        }

        // PUT: api/Bookings/5
        [HttpPut("{email}")]
        public async Task<IActionResult> Update([FromRoute] string email, [FromBody] AccBooking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(booking).State = EntityState.Modified;
                Console.WriteLine("State change, yet to save.");
                await _context.SaveChangesAsync();
                Console.WriteLine("Saved.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookingExists(booking.UserId))
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
            }

            return NoContent();
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<IActionResult> New([FromBody] AccBooking booking)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name.Equals(booking.UserId.ToString()) || User.IsInRole("Admin"))
                try
                {
                    _context.AccBooking.Add(booking);
                    await _context.SaveChangesAsync();

                    /// Need to inner join details inorder to create receipt

                    var detail = _context.AccDetail.Where(s => s.DetailId.Equals(booking.DetailId))
                                    .Include(s=>s.Prop).Include(s=>s.Prop.Acc);

                    var user = await _userDB.User.SingleOrDefaultAsync(s => s.UserId.Equals(booking.UserId));

                    if (detail.First().AvailableRooms > 0)
                        detail.First().AvailableRooms -= booking.RoomsBooked;
                    else
                        throw new Exception("No rooms available for booking to proceed!");
                    
                    if (await TryUpdateModelAsync<AccDetail>(detail.First()))
                    {
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateException  ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    EmailMessage message = new EmailMessage("Accommodation Booking.",
                        "Hi " + user.Name + ",<br/><br/>" +
                        "You have just booked for an accommodation using our a web services, the full details of the booking are: <br/>" +
                        detail.First().Prop.Acc.Country + "<br/>" + detail.First().Prop.Acc.Location + "<br/>" +
                        detail.First().Prop.PropName + "<br/>Booking date: " + booking.BookDate + "<br/>Number of nights booked: " +
                        booking.NumOfNights + "<br/>Number of rooms booked: " + booking.RoomsBooked + "<br/>Total: R " + booking.Total +
                        "<br/><br/>Kind Regards,<br/>Booking.com");

                    message.FromAddresses.Add(new EmailAddress("BookingServer.com", "validtest.r.me@gmail.com"));
                    message.ToAddresses.Add(new EmailAddress(user.Name, user.Email));

                    new Send(message, _emailConfiguration);

                    await _hubContext.Clients.All.BroadcastMessage("A user has just book for "
                        + detail.First().Prop.PropName + ", " + detail.First().AvailableRooms + " left.");

                    return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.InnerException);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.Source);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.TargetSite);
                    if (ex.Message.Contains("No rooms available"))
                        return NotFound("Rooms are unavailable.");
                    //Console.WriteLine(booking.PropId + " total" + booking.Total);
                    return BadRequest("Internal error.");
                }
            
            return Unauthorized();
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{UserId}"), Authorize(Policy = "Administrator")]
        public async Task<IActionResult> Delete([FromRoute] int UserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.AccBooking.SingleOrDefaultAsync(m => m.UserId.Equals(UserId));
            if (booking == null)
            {
                return NotFound();
            }

            try
            {
                _context.AccBooking.Remove(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                return NoContent();
            }

            return Ok(booking);
        }

        private bool BookingExists(int id)
        {
            return _context.AccBooking.Any(e => e.BookingId == id);
        }
    }
}