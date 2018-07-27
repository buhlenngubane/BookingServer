using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookingServer.Models.Users;
using BookingServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

namespace BookingServer.Controllers.Service
{
    [Produces("application/json")]
    [Route("api/Token/[action]")]
    public class TokenController : Controller
    {
        private readonly UserDBContext _context;
        private readonly JwtSettings _options;
        private readonly ITokenManager _tokenManager;

        public TokenController(UserDBContext context,
            IOptions<JwtSettings> optionsAccessor, ITokenManager tokenManager)
        {
            _context = context;
            _options = optionsAccessor.Value;
            _tokenManager = tokenManager;
        }

        [HttpGet(), Authorize]
        public async Task<IActionResult> SignIn()
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.UserId.Equals(int.Parse(User.Identity.Name)));
            return user != null ? Ok(user) : Ok("No user found for token.");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Refresh()
        {
            IActionResult response = BadRequest("Unable to refresh token!");
            try
            {
                var user = await _context.User.SingleOrDefaultAsync(m => m.UserId.Equals(int.Parse(User.Identity.Name)));
                
                await _tokenManager.DeactivateCurrentAsync();

                Console.WriteLine("Deactivating user" + User.Claims + " UserId : "+ User.Identity.Name);

                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
                Console.WriteLine("New Token for userId: "+ User.Identity.Name + " token: " + tokenString);
                return response;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Log.Error("Error refreshing userId: " + User.Identity.Name + " ", ex);
                return response;
            }
            
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
                    Log.Debug(string.Format("Creating Token, email : " + email));
                    if (user == null || userP == null)
                    {
                        return user == null ? 
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
                    Log.Error("Error creating token for " + email + " ", ex);
                }

                return response;

            }
            return BadRequest(ModelState);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> LogOut()
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _tokenManager.DeactivateCurrentAsync();

                    return Ok("LoggedOut :)");
                } catch(Exception ex)
                {
                    Log.Error("Redis possibly not functioning correctly. ", ex);
                    return BadRequest("Error, no token available for logout :(");
                }
            }

            return BadRequest("ModelState error! Not LoggedOut. " + ModelState);
        }

        private string BuildToken(User user)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                string role = "";

                if (user.Admin.Equals(true))
                    role = "Admin";
                else
                    role = "Customer";

                Claim[] claims = new[]
                {
                    new Claim(ClaimTypes.Name,user.UserId.ToString()),
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
                Console.WriteLine("Token error: " + ex);
                Log.Error("Token error : ",ex);
                throw ex;
            }
        }

        private JsonResult Error(string message)
        {
            return new JsonResult(message) { StatusCode = 400 };
        }

    }
}