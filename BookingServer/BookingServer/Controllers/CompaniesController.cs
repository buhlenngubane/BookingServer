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
    [Route("api/Companies/[action]")]
    public class CompaniesController : Controller
    {
        private readonly FlightDBContext _context;

        public CompaniesController(FlightDBContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        public IEnumerable<Company> GetAll()
        {
            return _context.Company;
        }

        // GET: api/Companies/5
        [HttpGet("{ids}")]
        public async Task<IActionResult> GetCompany([FromRoute] string ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Task<Company>> cmp = new List<Task<Company>>();

            if(!String.IsNullOrWhiteSpace(ids))
            {
                var list = ids?.Split(",").Select(int.Parse).ToArray();
                try
                {
                    foreach (int lst in list)
                    {
                        cmp.Add(_context.Company.SingleAsync(m => m.Cid.Equals(lst)));
                    }
                    if (cmp == null)
                    {
                        return NotFound();
                    }
                    return Ok(cmp);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error in loop: " + ex.Message);
                    return NotFound();
                }
            }

            return NotFound();
            
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany([FromRoute] int id, [FromBody] Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != company.Cid)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST: api/Companies
        [HttpPost]
        public async Task<IActionResult> PostCompany([FromBody] Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.Cid }, company);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _context.Company.SingleOrDefaultAsync(m => m.Cid == id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(company);
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Cid == id);
        }
    }
}