using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Aaronbackend.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class LogicController : ControllerBase
    {
        private readonly DBclass _context;

        public LogicController(DBclass context)
        {
            _context = context;
        }

        // ✅ Welcome API
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new { message = "Welcome to the API" }); // 🔹 JSON response for consistency
        }

        // ✅ Create a new vehicle
        [HttpPost("add")]
        public async Task<ActionResult> PostVehicle([FromBody] Vehicle vehicle)
        {
            if (vehicle == null)
                return BadRequest("Vehicle cannot be null.");

            if (string.IsNullOrEmpty(vehicle.LicensePlate) ||
                string.IsNullOrEmpty(vehicle.Model) ||
                string.IsNullOrEmpty(vehicle.Owner))
            {
                return BadRequest("All fields must be filled.");
            }

            vehicle.RegistrationDate = vehicle.RegistrationDate == default ? DateTime.Now : vehicle.RegistrationDate;

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
        }

        // ✅ Get vehicles with pagination
        [HttpGet("vehicles")]
        public async Task<ActionResult> GetVehicles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var totalRecords = await _context.Vehicles.CountAsync();
            var vehicles = await _context.Vehicles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (vehicles.Count == 0)
                return NoContent();

            return Ok(new
            {
                TotalCount = totalRecords,
                Vehicles = vehicles
            });
        }

        // ✅ Get a specific vehicle by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound($"Vehicle with ID {id} not found.");

            return Ok(vehicle);
        }

        // ✅ Delete a vehicle by ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound($"Vehicle with ID {id} not found.");

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent(); // 🔹 Best practice: 204 No Content for deletions
        }

        // ✅ Update a vehicle by ID
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutVehicle(int id, [FromBody] Vehicle vehicle)
        {
            if (vehicle == null || id != vehicle.Id)
                return BadRequest("Invalid vehicle data.");

            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
                return NotFound($"Vehicle with ID {id} not found.");

            // Update fields
            existingVehicle.LicensePlate = vehicle.LicensePlate;
            existingVehicle.Model = vehicle.Model;
            existingVehicle.Owner = vehicle.Owner;
            existingVehicle.RegistrationDate = vehicle.RegistrationDate == default ? DateTime.Now : vehicle.RegistrationDate;
            existingVehicle.VehicleName = vehicle.VehicleName;
            existingVehicle.OwnerContactNumber = vehicle.OwnerContactNumber;
            existingVehicle.OwnerAddress = vehicle.OwnerAddress;
            existingVehicle.Price = vehicle.Price;
            existingVehicle.OwnerEmail = vehicle.OwnerEmail;

            await _context.SaveChangesAsync();

            return Ok(existingVehicle);
        }
    }
}
