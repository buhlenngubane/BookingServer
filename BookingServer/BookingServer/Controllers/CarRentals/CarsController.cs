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

        [HttpGet("{id}"), Authorize(Policy="Administrator")]
        public async Task<IActionResult> GetAllCars([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var carRental = _context.Car.Where(s => s.Cmp.Crent.CrentId.Equals(id))
                    .Include(s=>s.CarBooking)
                    .Include(s=>s.Cmp)
                        .ThenInclude(s=>s.Crent)
                    .Include(s=>s.Ctype);

                if (!carRental.Any())
                    return Ok("No carRental for id " + id);

                foreach(Car c in carRental)
                {
                    c.Ctype.Picture = null;
                    c.Cmp.Picture = null;
                    c.Cmp.Crent.Ccompany = null;
                    c.Ctype.Car = null;
                    c.Cmp.Car = null;
                }

                return Ok(await carRental.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
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

            
            if (!list.Any())
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