using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using BookingServer.Models.Email;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using BookingServer.Services.Email;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using System.IO;
using MailKit.Security;
using Serilog;


namespace BookingServer.Controllers.Users
{
    [Produces("application/json")]
    [Route("api/Users/[action]")]
    public class UsersController : Controller
    {
        private readonly UserDBContext _context;
        private readonly JwtSettings _options;
        private readonly IEmailConfiguration _emailConfiguration;

        public UsersController(UserDBContext context,
            IOptions<JwtSettings> optionsAccessor,
            IEmailConfiguration emailConfiguration)
        {
            _context = context;
            _options = optionsAccessor.Value;
            _emailConfiguration = emailConfiguration;

        }
        // GET: api/Users
        [HttpGet, Authorize(Policy = "Administrator")]
        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        // GET: api/Users/5
        [HttpGet("{email}&{password}")]
        public async Task<IActionResult> GetUser([FromRoute] string email,string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Console.WriteLine("Password: " + password);
            var user = await _context.User.SingleOrDefaultAsync(m => m.Email.Equals(email));
            var userp = await _context.User.SingleOrDefaultAsync(m => m.Password == password);

            try
            {
                if (user==null || userp==null)
                {
                    if (user == null && userp == null)
                        return NotFound("User not registered.");

                    return user==null ? 
                        NotFound("Email not found") : NotFound("Password not found for account");
                }
                else if(!user.UserId.Equals(userp.UserId))
                {
                    throw new Exception("User tried to log in with incorrect email password combination.");
                }
            }
            catch(Exception ex)
            {
                Log.Warning(ex + "");
                Console.WriteLine("Error : "+ex);
                return NotFound("Password not found for account");
            }
            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UserEmailExists(user.Email))
            {
                var saveUser = await _context.User.SingleOrDefaultAsync(m=>m.Email.Equals(user.Email));
                /*if(user.Password.Equals("Default@1234Booking"))
                {
                    user.Password = saveUser.Password;
                }*/

                try
                {
                    _context.Entry(saveUser).State = EntityState.Modified;
                    Console.WriteLine("State change, yet to save.");
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Saved.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("Error updating: "+ ex);
                    Log.Error("" + ex);
                }
                catch (Exception ex)
                {
                    Log.Error("" + ex);
                }
                
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid || UserEmailExists(user.Email))
            {
                return !ModelState.IsValid?BadRequest(ModelState):BadRequest("Email exists");
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId });
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}"),Authorize]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.UserId.Equals(id));
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user.Name);
        }

        private bool UserIdExists(int id)
        {
            return _context.User.Any(e => e.UserId.Equals(id));
        }

        private bool UserEmailExists(string email)
        {
            return _context.User.Any(e => e.Email.Equals(email));
        }

        private bool UserExists(int id, string email)
        {
            return _context.User.Any(e => e.UserId.Equals(id) && e.Email.Equals(email));
        }

        
    }
}