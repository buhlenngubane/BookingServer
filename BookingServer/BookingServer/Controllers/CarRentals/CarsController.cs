using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingServer.Models.CarRentals;
using Microsoft.AspNetCore.Authorization;

namespace BookingServer.Controllers.CarRentals
{
    [Produces("application/json")]
    [Route("api/CarRentals/Cars/[action]")]
    public class CarsController : Controller
    {
        private readonly CarRentalDBContext _context;

        public CarsController(CarRentalDBContext context)
        {
            _context = context;
        }

        // GET: api/Cars
        [HttpGet]
        public IEnumerable<Car> GetAll()
        {
            return _context.Car;
        }

        [HttpGet, Authorize(Policy="Administrator")]
        public IEnumerable<Car> GetAllCars()
        {
            return _context.Car.Include(s => s.Cmp.Crent).Include(s => s.Ctype).Include(s => s.CarBooking);
        }

        // GET: api/Cars/5
        [HttpGet("{search}")]
        public async Task<IActionResult> GetDetails([FromRoute] string search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var list=_context.Car.Include(c => c.Ctype)
                .Include(c=>c.Cmp).AsNoTracking()
                .Where(m => m.Cmp.Crent.Location.Equals(search)).OrderBy(s=>s.Price);

            //list.Load();
            

            if (list.Equals(null))
            {
                return NotFound();
            }

            return Ok(await list.ToListAsync());
        }

        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar([FromRoute] int id, [FromBody] Car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != car.CarId)
            {
                return BadRequest();
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
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

        // POST: api/Cars
        [HttpPost]
        public async Task<IActionResult> PostCar([FromBody] Car car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Car.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCar", new { id = car.CarId }, car);
        }

        // DELETE: api/Cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var car = await _context.Car.SingleOrDefaultAsync(m => m.CarId == id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Car.Remove(car);
            await _context.SaveChangesAsync();

            return Ok(car);
        }

        private bool CarExists(int id)
        {
            return _context.Car.Any(e => e.CarId == id);
        }
    }
}