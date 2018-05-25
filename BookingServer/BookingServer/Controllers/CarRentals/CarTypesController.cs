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
    [Route("api/CarRentals/CarTypes/[action]")]
    public class CarTypesController : Controller
    {
        private readonly CarRentalDBContext _context;

        public CarTypesController(CarRentalDBContext context)
        {
            _context = context;
        }

        // GET: api/CarTypes
        [HttpGet]
        public IEnumerable<CarType> GetCarType()
        {
            return _context.CarType;
        }

        // GET: api/CarTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carType = await _context.CarType.SingleOrDefaultAsync(m => m.CtypeId == id);

            if (carType == null)
            {
                return NotFound();
            }

            return Ok(carType);
        }

        // PUT: api/CarTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarType([FromRoute] int id, [FromBody] CarType carType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != carType.CtypeId)
            {
                return BadRequest();
            }

            _context.Entry(carType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarTypeExists(id))
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

        // POST: api/CarTypes
        [HttpPost]
        public async Task<IActionResult> PostCarType([FromBody] CarType carType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CarType.Add(carType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarType", new { id = carType.CtypeId }, carType);
        }

        [HttpPost]
        public async Task<IActionResult> PostCarTypes([FromBody] CarType[] carTypes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CarType.AddRange(carTypes);
            await _context.SaveChangesAsync();

            return Created("api/CarRentals/CarTypes/PostCarTypes", carTypes);
        }

        // DELETE: api/CarTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var carType = await _context.CarType.SingleOrDefaultAsync(m => m.CtypeId == id);
            if (carType == null)
            {
                return NotFound();
            }

            _context.CarType.Remove(carType);
            await _context.SaveChangesAsync();

            return Ok(carType);
        }

        private bool CarTypeExists(int id)
        {
            return _context.CarType.Any(e => e.CtypeId == id);
        }
    }
}