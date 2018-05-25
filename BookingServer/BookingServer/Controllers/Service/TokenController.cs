using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookingServer.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace BookingServer.Controllers.Service
{
    [Produces("application/json")]
    [Route("api/Token")]
    public class TokenController : Controller
    {
        private readonly UserDBContext _context;
        private readonly JwtSettings _options;

        public TokenController(UserDBContext context,
            IOptions<JwtSettings> optionsAccessor)
        {
            _context = context;
            _options = optionsAccessor.Value;
        }

        [HttpGet(), Authorize]
        public async Task<IActionResult> SignIn()
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Email.Equals(User.Identity.Name));
            //User sendUser = new User(user.Name,user.Email);
            user.Password = "";
            //var userName = User.Identity.Name;
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("{email}&{password}")]
        public async Task<IActionResult> CreateToken([FromRoute]string email, string password)
        {
            if (ModelState.IsValid)
            {
                IActionResult response = Unauthorized();
                var user = await _context.User.SingleOrDefaultAsync(m => m.Email.Equals(email));
                var userP = await _context.User.SingleOrDefaultAsync(m =>  m.Password.Equals(password));

                try
                { 
                    if (user.Equals(null) || userP.Equals(null))
                    {
                        return user.Equals(null) ? 
                            NotFound("Email not found") : NotFound("Password not found");
                    }
                    else if (!user.UserId.Equals(userP.UserId))
                    {
                        return BadRequest();
                    }

                    var tokenString = BuildToken(user);
                        response = Ok(new { token = tokenString });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Error creating token :" + ex);
                }

                return response;

            }
            return Error("Unknown error! ");
        }

        private string BuildToken(User user)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                string role = "";

                if (user.UserId.Equals(1))
                    role = "Admin";
                else
                    role = "Customer";

                Claim[] claims = new[]
                {
                    new Claim(ClaimTypes.Name,user.Email),
                    new Claim(ClaimTypes.GroupSid,user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, role)
                    
                };

                var token = new JwtSecurityToken(_options.Issuer,
                  _options.Audience,claims,
                  expires: DateTime.Now.AddMinutes(_options.ExpiryMinutes),
                  signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch(Exception ex)
            {
                return "Error occured: "+ex;
            }
        }

        private JsonResult Error(string message)
        {
            return new JsonResult(message) { StatusCode = 400 };
        }

    }
}