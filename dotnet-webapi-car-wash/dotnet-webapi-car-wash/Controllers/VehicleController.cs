using dotnet_webapi_car_wash.Models;
using dotnet_webapi_car_wash.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_car_wash.Controllers
{
    [Route("api/Vehicle")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private static List<Customer> customers = CustomerController.customers;

        public static List<Vehicle> vehicles = new List<Vehicle>
{
    new Vehicle
    {
        LicensePlate = "ABC123",
        Brand = "Toyota",
        Model = "Camry",
        Traction = "FWD",
        Color = "Blue",
        LastServiceDate = new DateTime(2024, 1, 15),
        HasNanoCeramicTreatment = true,
        CustomerId = "123456789"
    },
    new Vehicle
    {
        LicensePlate = "DEF456",
        Brand = "Honda",
        Model = "Civic",
        Traction = "FWD",
        Color = "Red",
        LastServiceDate = new DateTime(2023, 12, 10), // Hace más tiempo
        HasNanoCeramicTreatment = false,
        CustomerId = "987654321"
    },
    new Vehicle
    {
        LicensePlate = "GHI789",
        Brand = "Nissan",
        Model = "Sentra",
        Traction = "FWD",
        Color = "White",
        LastServiceDate = new DateTime(2024, 2, 5),
        HasNanoCeramicTreatment = true,
        CustomerId = "456789123"
    },
    new Vehicle
    {
        LicensePlate = "JKL012",
        Brand = "Ford",
        Model = "Focus",
        Traction = "FWD",
        Color = "Black",
        LastServiceDate = new DateTime(2023, 11, 20), // Hace mucho tiempo
        HasNanoCeramicTreatment = false,
        CustomerId = "321654987"
    },
    new Vehicle
    {
        LicensePlate = "MNO345",
        Brand = "Hyundai",
        Model = "Elantra",
        Traction = "FWD",
        Color = "Silver",
        LastServiceDate = new DateTime(2024, 1, 30),
        HasNanoCeramicTreatment = true,
        CustomerId = "789456123"
    },
    new Vehicle
    {
        LicensePlate = "PQR678",
        Brand = "Kia",
        Model = "Rio",
        Traction = "FWD",
        Color = "Blue",
        LastServiceDate = new DateTime(2023, 10, 15), // Hace mucho tiempo
        HasNanoCeramicTreatment = false,
        CustomerId = "789456123" // Segundo vehículo para Luis
    },
    new Vehicle
    {
        LicensePlate = "STU901",
        Brand = "Chevrolet",
        Model = "Spark",
        Traction = "FWD",
        Color = "Yellow",
        LastServiceDate = null, // Nunca ha tenido servicio
        HasNanoCeramicTreatment = false,
        CustomerId = "456789123" // Segundo vehículo para Carlos
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

                // Enrich vehicles with customer information
                var enrichedVehicles = vehicles.Select(v =>
                {
                    var customer = customers.FirstOrDefault(c => c.IdNumber == v.CustomerId);
                    return new Vehicle
                    {
                        LicensePlate = v.LicensePlate,
                        Brand = v.Brand,
                        Model = v.Model,
                        Traction = v.Traction,
                        Color = v.Color,
                        LastServiceDate = v.LastServiceDate,
                        HasNanoCeramicTreatment = v.HasNanoCeramicTreatment,
                        CustomerId = v.CustomerId,
                        Customer = customer
                    };
                }).ToList();

                return Ok(enrichedVehicles);
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

                // Enrich with customer information
                var customer = customers.FirstOrDefault(c => c.IdNumber == vehicle.CustomerId);
                vehicle.Customer = customer;

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
                        v.CustomerId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (v.LastServiceDate?.ToString("dd/MM/yyyy").Contains(searchTerm) ?? false) ||
                        (v.HasNanoCeramicTreatment.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Enrich with customer information
                var enrichedVehicles = filteredVehicles.Select(v =>
                {
                    var customer = customers.FirstOrDefault(c => c.IdNumber == v.CustomerId);
                    return new Vehicle
                    {
                        LicensePlate = v.LicensePlate,
                        Brand = v.Brand,
                        Model = v.Model,
                        Traction = v.Traction,
                        Color = v.Color,
                        LastServiceDate = v.LastServiceDate,
                        HasNanoCeramicTreatment = v.HasNanoCeramicTreatment,
                        CustomerId = v.CustomerId,
                        Customer = customer
                    };
                }).ToList();

                return Ok(enrichedVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching vehicles", error = ex.Message });
            }
        }

        // GET: api/Vehicle/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public ActionResult<IEnumerable<Vehicle>> GetByCustomer(string customerId)
        {
            try
            {
                var customerVehicles = vehicles.Where(v => v.CustomerId == customerId).ToList();

                // Enrich with customer information
                var customer = customers.FirstOrDefault(c => c.IdNumber == customerId);
                var enrichedVehicles = customerVehicles.Select(v =>
                {
                    v.Customer = customer;
                    return v;
                }).ToList();

                return Ok(enrichedVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving vehicles by customer", error = ex.Message });
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
                if (string.IsNullOrWhiteSpace(vehicle.CustomerId))
                {
                    validationErrors.Add("Customer is required.");
                }

                // Check if vehicle with same license plate already exists
                var existingVehicle = GetVehicleById(vehicle.LicensePlate);
                if (existingVehicle != null)
                {
                    validationErrors.Add("A vehicle with that license plate already exists.");
                }

                // Validate customer exists
                var customer = customers.FirstOrDefault(c => c.IdNumber == vehicle.CustomerId);
                if (customer == null)
                {
                    validationErrors.Add("The selected customer does not exist.");
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

                vehicle.Customer = customer;
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
                if (string.IsNullOrWhiteSpace(vehicle.CustomerId))
                {
                    validationErrors.Add("Customer is required.");
                }

                // Validate customer exists
                var customer = customers.FirstOrDefault(c => c.IdNumber == vehicle.CustomerId);
                if (customer == null)
                {
                    validationErrors.Add("The selected customer does not exist.");
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
                vehicle.Customer = customer;
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
            return vehicles.FirstOrDefault(v => v.LicensePlate == id);
        }

        private bool UpdateVehicle(Vehicle updatedVehicle)
        {
            try
            {
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (vehicles[i].LicensePlate == updatedVehicle.LicensePlate)
                    {
                        vehicles[i] = updatedVehicle;
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool DeleteVehicle(string id)
        {
            try
            {
                var vehicleToRemove = GetVehicleById(id);
                if (vehicleToRemove != null)
                {
                    vehicles.Remove(vehicleToRemove);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}