using dotnet_webapi_car_wash.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_car_wash.Controllers
{
    [Route("api/Vehicle")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        // Static list to store vehicles in memory
        private static List<Vehicle> vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                LicensePlate = "ABC123",
                Brand = "Toyota",
                Model = "Camry",
                Traction = "FWD",
                Color = "Blue",
                LastServiceDate = new DateTime(2024, 1, 15),
                HasNanoCeramicTreatment = true
            }
        };

        // GET: api/Vehicle
        [HttpGet]
        public ActionResult<IEnumerable<Vehicle>> Get()
        {
            try
            {
                if (vehicles.Count == 0)
                {
                    return NotFound(new { message = "No vehicles found" });
                }
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving vehicles", error = ex.Message });
            }
        }

        // GET: api/Vehicle/{id}
        [HttpGet("{id}")]
        public ActionResult<Vehicle> Get(string id)
        {
            try
            {
                var vehicle = GetVehicleById(id);
                if (vehicle == null)
                {
                    return NotFound(new { message = $"Vehicle with license plate '{id}' not found" });
                }
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving vehicle", error = ex.Message });
            }
        }

        // GET: api/Vehicle/search
        [HttpGet("search")]
        public ActionResult<IEnumerable<Vehicle>> GetWithSearch(string searchTerm)
        {
            try
            {
                var filteredVehicles = vehicles;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filteredVehicles = vehicles.Where(v =>
                        v.LicensePlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        v.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        v.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        v.Traction.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        v.Color.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (v.LastServiceDate?.ToString("dd/MM/yyyy").Contains(searchTerm) ?? false) ||
                        (v.HasNanoCeramicTreatment.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                return Ok(filteredVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching vehicles", error = ex.Message });
            }
        }

        // POST: api/Vehicle
        [HttpPost]
        public ActionResult<Vehicle> Post([FromBody] Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    return BadRequest(new { message = "Vehicle data is required" });
                }

                var validationErrors = new List<string>();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(vehicle.LicensePlate))
                {
                    validationErrors.Add("License plate is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Brand))
                {
                    validationErrors.Add("Brand is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Model))
                {
                    validationErrors.Add("Model is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Traction))
                {
                    validationErrors.Add("Traction is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Color))
                {
                    validationErrors.Add("Color is required.");
                }

                // Check if vehicle with same license plate already exists
                var existingVehicle = GetVehicleById(vehicle.LicensePlate);
                if (existingVehicle != null)
                {
                    validationErrors.Add("A vehicle with that license plate already exists.");
                }

                // Validate last service date not in future
                if (vehicle.LastServiceDate.HasValue && vehicle.LastServiceDate.Value > DateTime.Now)
                {
                    validationErrors.Add("The last service date cannot be in the future.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                vehicles.Add(vehicle);
                return CreatedAtAction(nameof(Get), new { id = vehicle.LicensePlate }, vehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating vehicle", error = ex.Message });
            }
        }

        // PUT: api/Vehicle/{id}
        [HttpPut("{id}")]
        public ActionResult<Vehicle> Put(string id, [FromBody] Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    return BadRequest(new { message = "Vehicle data is required" });
                }

                var existingVehicle = GetVehicleById(id);
                if (existingVehicle == null)
                {
                    return NotFound(new { message = $"Vehicle with license plate '{id}' not found" });
                }

                var validationErrors = new List<string>();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(vehicle.Brand))
                {
                    validationErrors.Add("Brand is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Model))
                {
                    validationErrors.Add("Model is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Traction))
                {
                    validationErrors.Add("Traction is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Color))
                {
                    validationErrors.Add("Color is required.");
                }

                // Validate last service date not in future
                if (vehicle.LastServiceDate.HasValue && vehicle.LastServiceDate.Value > DateTime.Now)
                {
                    validationErrors.Add("The last service date cannot be in the future.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                // Ensure license plate doesn't change
                vehicle.LicensePlate = id;
                bool success = UpdateVehicle(vehicle);

                if (success)
                {
                    return Ok(vehicle);
                }
                else
                {
                    return StatusCode(500, new { message = "Could not update vehicle" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating vehicle", error = ex.Message });
            }
        }

        // DELETE: api/Vehicle/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            try
            {
                var vehicle = GetVehicleById(id);
                if (vehicle == null)
                {
                    return NotFound(new { message = $"Vehicle with license plate '{id}' not found" });
                }

                bool success = DeleteVehicle(id);

                if (success)
                {
                    return Ok(new { message = $"Vehicle with license plate '{id}' deleted successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Could not delete vehicle" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting vehicle", error = ex.Message });
            }
        }

        #region Private Helper Methods

        private Vehicle GetVehicleById(string id)
        {
            Vehicle vehicle = null;
            foreach (var veh in vehicles)
            {
                if (veh.LicensePlate == id)
                {
                    vehicle = veh;
                    break;
                }
            }
            return vehicle;
        }

        private bool UpdateVehicle(Vehicle updatedVehicle)
        {
            bool success = false;
            try
            {
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (vehicles[i].LicensePlate == updatedVehicle.LicensePlate)
                    {
                        vehicles[i] = updatedVehicle;
                        success = true;
                        break;
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        private bool DeleteVehicle(string id)
        {
            bool success = false;
            try
            {
                Vehicle vehicleToRemove = GetVehicleById(id);
                if (vehicleToRemove != null)
                {
                    vehicles.Remove(vehicleToRemove);
                    success = true;
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        #endregion
    }
}