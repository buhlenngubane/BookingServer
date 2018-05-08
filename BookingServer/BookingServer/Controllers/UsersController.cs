using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Users/[action]")]
    public class UsersController : Controller
    {
        private readonly UserDBContext _context;
        private readonly JwtSettings _options;

        public UsersController(UserDBContext context, IOptions<JwtSettings> optionsAccessor)
        {
            _context = context;
            _options = optionsAccessor.Value;
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

            var user = await _context.User.SingleOrDefaultAsync(m => m.Email == email);
            var userp = await _context.User.SingleOrDefaultAsync(m => m.Password == password);

            try
            {
                if (user.Equals(null) || userp.Equals(null))
                {
                    return user.Equals(null) ? 
                        NotFound("Email not found") : NotFound("Password not found");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error : "+ex.Message);
                return BadRequest();
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

            if (UserIdExists(user.UserId))
            {
                try
                {
                    _context.Entry(user).State = EntityState.Modified;
                    Console.WriteLine("State change, yet to save.");
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Saved.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("Error updating: "+ ex);
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

            //if(UserExists(user.Email))
               // return Bad

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] User userRequest)
        {
            //Console.WriteLine("Models=State passed: " + userRequest.Email);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Console.WriteLine("Models=State passed: "+ userRequest.Email);
            try
            {
                var user = await _context.User.SingleOrDefaultAsync(m => m.Email == userRequest.Email);
                var userp = await _context.User.SingleOrDefaultAsync(m => m.Password == userRequest.Password);

                if (user.Equals(null) || userp.Equals(null))
                {
                    return user.Equals(null) ? NotFound("Email not found") : NotFound("Password not found");
                }
                return Ok(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error occured: "+ ex);
                return BadRequest("Error occured: " + ex);
            }
            

            
        }

        // DELETE: api/Users/5
        [HttpDelete("{email}"),Authorize]
        public async Task<IActionResult> DeleteUser([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
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