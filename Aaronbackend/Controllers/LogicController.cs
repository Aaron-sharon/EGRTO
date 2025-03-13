using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aaronbackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogicController : ControllerBase
    {
        private readonly DBclass _context;
        private readonly IValidator<Vehicle> _vehicleValidator;

        public LogicController(DBclass context, IValidator<Vehicle> vehicleValidator)
        {
            _context = context;
            _vehicleValidator = vehicleValidator;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new { message = "Welcome to the API" });
        }

        [HttpPost("add")]
        public async Task<ActionResult> PostVehicle([FromBody] Vehicle vehicle)
        {
            ValidationResult result = await _vehicleValidator.ValidateAsync(vehicle);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            vehicle.RegistrationDate = vehicle.RegistrationDate == default ? DateTime.Now : vehicle.RegistrationDate;

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
        }

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

        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> GetVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound($"Vehicle with ID {id} not found.");

            return Ok(vehicle);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound($"Vehicle with ID {id} not found.");

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutVehicle(int id, [FromBody] Vehicle vehicle)
        {
            if (vehicle == null || id != vehicle.Id)
                return BadRequest("Invalid vehicle data.");

            ValidationResult result = await _vehicleValidator.ValidateAsync(vehicle);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var existingVehicle = await _context.Vehicles.FindAsync(id);
            if (existingVehicle == null)
                return NotFound($"Vehicle with ID {id} not found.");

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