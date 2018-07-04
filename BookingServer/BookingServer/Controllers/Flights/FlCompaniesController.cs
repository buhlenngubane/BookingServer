using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.Flights;

namespace BookingServer.Controllers.Flights
{
    [Produces("application/json")]
    [Route("api/Flights/FlCompanies/[action]")]
    public class FlCompaniesController : Controller
    {
        private readonly FlightDBContext _context;

        public FlCompaniesController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/FlCompanies
        [HttpGet]
        public IEnumerable<FlCompany> GetFlCompany()
        {
            return _context.FlCompany;
        }

        // GET: api/FlCompanies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlCompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _context.FlCompany.SingleOrDefaultAsync(m => m.Cid == id);

            if (company == null)
            {
                return Ok("No company found with id " + id);
            }

            return Ok(company);
        }

        // PUT: api/FlCompanies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlCompany([FromRoute] int id, [FromBody] FlCompany flCompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != flCompany.Cid)
            {
                return BadRequest();
            }

            _context.Entry(flCompany).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlCompanyExists(id))
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

        // POST: api/FlCompanies
        [HttpPost]
        public async Task<IActionResult> PostFlCompany([FromBody] FlCompany flCompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FlCompany.Add(flCompany);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlCompany", new { id = flCompany.Cid }, flCompany);
        }

        // DELETE: api/FlCompanies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlCompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flCompany = await _context.FlCompany.SingleOrDefaultAsync(m => m.Cid == id);
            if (flCompany == null)
            {
                return NotFound();
            }

            _context.FlCompany.Remove(flCompany);
            await _context.SaveChangesAsync();

            return Ok(flCompany);
        }

        private bool FlCompanyExists(int id)
        {
            return _context.FlCompany.Any(e => e.Cid == id);
        }
    }
}