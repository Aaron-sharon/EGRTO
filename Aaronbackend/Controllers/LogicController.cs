using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aaronbackend.Controllers
{
    [ApiController]
    [Route("[controller]")] // 🔹 Better practice: Use "api/logic" for clarity
    public class LogicController : ControllerBase // 🔹 Use ControllerBase for API controllers
    {
        private readonly DBclass _context;

        public LogicController(DBclass context)
        {
            _context = context;
        }

        // ✅ Remove "Index()" OR properly define it if needed
        [HttpGet("index")] // 🔹 Explicit route to avoid conflicts
        public IActionResult Index()
        {
            return Ok("Welcome to the API"); // 🔹 Return JSON response instead of View()
        }

        // ✅ Create a new vehicle
        [HttpPost("add")]
        public async Task<ActionResult<string>> PostVehicle([FromBody] Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return BadRequest("Vehicle cannot be null.");
            }

            if (string.IsNullOrEmpty(vehicle.LicensePlate) ||
                string.IsNullOrEmpty(vehicle.Model) ||
                string.IsNullOrEmpty(vehicle.Owner))
            {
                return BadRequest("All fields must be filled.");
            }

            if (vehicle.RegistrationDate == default(DateTime))
            {
                vehicle.RegistrationDate = DateTime.Now;
            }

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return Ok("Request Submitted");
        }

        // ✅ Get all vehicles
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetVehicles()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            if (vehicles == null || vehicles.Count == 0)
            {
                return NotFound("No vehicles found.");
            }
            return Ok(vehicles);
        }

        // ✅ Get a specific vehicle by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID {id} not found.");
            }
            return Ok(vehicle);
        }

        // ✅ Delete a vehicle by ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID {id} not found.");
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent(); // 🔹 Best practice: Return 204 No Content on deletion
        }

        [HttpPut("update/{id}")] // Define route
        public async Task<IActionResult> PutVehicle(int id, [FromBody] Vehicle vehicle)
        {
            if (vehicle == null || id != vehicle.Id)
            {
                return BadRequest("Invalid vehicle data.");
            }

            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
            {
                return NotFound($"Vehicle with ID {id} not found.");
            }

            // Update the existing vehicle with new data
            existingVehicle.LicensePlate = vehicle.LicensePlate;
            existingVehicle.Model = vehicle.Model;
            existingVehicle.Owner = vehicle.Owner;
            existingVehicle.RegistrationDate = vehicle.RegistrationDate == default ? DateTime.Now : vehicle.RegistrationDate;
            existingVehicle.VehicleName = vehicle.VehicleName;
            existingVehicle.OwnerContactNumber = vehicle.OwnerContactNumber;
            existingVehicle.OwnerAddress = vehicle.OwnerAddress;
            existingVehicle.Price = vehicle.Price;
            existingVehicle.OwnerEmail = vehicle.OwnerEmail;

            await _context.SaveChangesAsync(); // Save changes

            return Ok(existingVehicle); // Return updated vehicle
        }

    }
}
