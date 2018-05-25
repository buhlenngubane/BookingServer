using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.CarRentals;

namespace BookingServer.Controllers.CarRentals
{
    [Produces("application/json")]
    [Route("api/CarRentals/Ccompanies/[action]")]
    public class CcompaniesController : Controller
    {
        private readonly CarRentalDBContext _context;

        public CcompaniesController(CarRentalDBContext context)
        {
            _context = context;
        }

        // GET: api/Ccompanies
        [HttpGet]
        public IEnumerable<Ccompany> GetCcompany()
        {
            return _context.Ccompany;
        }

        // GET: api/Ccompanies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCcompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ccompany = await _context.Ccompany.SingleOrDefaultAsync(m => m.CmpId == id);

            if (ccompany == null)
            {
                return NotFound();
            }

            return Ok(ccompany);
        }

        // PUT: api/Ccompanies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCcompany([FromRoute] int id, [FromBody] Ccompany ccompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ccompany.CmpId)
            {
                return BadRequest();
            }

            _context.Entry(ccompany).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CcompanyExists(id))
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

        // POST: api/Ccompanies
        [HttpPost]
        public async Task<IActionResult> PostCcompany([FromBody] Ccompany ccompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Ccompany.Add(ccompany);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCcompany", new { id = ccompany.CmpId }, ccompany);
        }

        [HttpPost]
        public async Task<IActionResult> PostCcompanys([FromBody] Ccompany[] ccompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Ccompany.AddRange(ccompany);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostCcompanys", ccompany);
        }

        // DELETE: api/Ccompanies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCcompany([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ccompany = await _context.Ccompany.SingleOrDefaultAsync(m => m.CmpId == id);
            if (ccompany == null)
            {
                return NotFound();
            }

            _context.Ccompany.Remove(ccompany);
            await _context.SaveChangesAsync();

            return Ok(ccompany);
        }

        private bool CcompanyExists(int id)
        {
            return _context.Ccompany.Any(e => e.CmpId == id);
        }
    }
}