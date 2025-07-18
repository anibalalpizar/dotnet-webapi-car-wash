using dotnet_webapi_car_wash.Models;
using dotnet_webapi_car_wash.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_car_wash.Controllers
{
    [Route("api/CarWash")]
    [ApiController]
    public class CarWashController : ControllerBase
    {
        private static List<Customer> customers = CustomerController.customers;
        private static List<Vehicle> vehicles = VehicleController.vehicles;

        public static List<CarWash> carWashs = new List<CarWash>
        {
            new CarWash
            {
                IdCarWash = "CW001",
                VehicleLicensePlate = "ABC123",
                IdClient = "123456789",
                IdEmployee = "EMP001",
                WashType = WashType.Premium,
                WashStatus = WashStatus.Billed,
                CreationDate = DateTime.Now.AddDays(-10),
                TotalPrice = 25000,
                Observations = "Cliente satisfecho con el servicio premium"
            },
            new CarWash
            {
                IdCarWash = "CW002",
                VehicleLicensePlate = "GHI789",
                IdClient = "456789123",
                IdEmployee = "EMP002",
                WashType = WashType.Basic,
                WashStatus = WashStatus.Billed,
                CreationDate = DateTime.Now.AddDays(-5),
                TotalPrice = 15000,
                Observations = "Lavado básico completado"
            },

            new CarWash
            {
                IdCarWash = "CW003",
                VehicleLicensePlate = "DEF456",
                IdClient = "987654321",
                IdEmployee = "EMP001",
                WashType = WashType.Deluxe,
                WashStatus = WashStatus.Billed,
                CreationDate = DateTime.Now.AddDays(-45),
                TotalPrice = 30000,
                Observations = "Lavado deluxe"
            },
            new CarWash
            {
                IdCarWash = "CW004",
                VehicleLicensePlate = "JKL012",
                IdClient = "321654987",
                IdEmployee = "EMP003",
                WashType = WashType.LaJoya,
                WashStatus = WashStatus.Billed,
                CreationDate = DateTime.Now.AddDays(-60),
                TotalPrice = 50000,
                Observations = "Servicio La Joya con tratamiento especial"
            },
            new CarWash
            {
                IdCarWash = "CW005",
                VehicleLicensePlate = "PQR678",
                IdClient = "789456123",
                IdEmployee = "EMP002",
                WashType = WashType.Basic,
                WashStatus = WashStatus.Billed,
                CreationDate = DateTime.Now.AddDays(-50),
                TotalPrice = 15000,
                Observations = "Lavado básico"
            },
            new CarWash
            {
                IdCarWash = "CW006",
                VehicleLicensePlate = "ABC123",
                IdClient = "123456789",
                IdEmployee = "EMP001",
                WashType = WashType.Deluxe,
                WashStatus = WashStatus.Billed,
                CreationDate = DateTime.Now.AddDays(-80),
                TotalPrice = 30000,
                Observations = "Lavado anterior del mismo vehículo"
            },
            new CarWash
            {
                IdCarWash = "CW007",
                VehicleLicensePlate = "MNO345",
                IdClient = "789456123",
                IdEmployee = "EMP003",
                WashType = WashType.Premium,
                WashStatus = WashStatus.Billed,
                CreationDate = DateTime.Now.AddDays(-35),
                TotalPrice = 25000,
                Observations = "Lavado premium con nano cerámico"
            },
            new CarWash
            {
                IdCarWash = "CW008",
                VehicleLicensePlate = "GHI789",
                IdClient = "456789123",
                IdEmployee = "EMP002",
                WashType = WashType.Deluxe,
                WashStatus = WashStatus.InProgress,
                CreationDate = DateTime.Now.AddDays(-1),
                TotalPrice = 30000,
                Observations = "Lavado en proceso"
            },
            new CarWash
            {
                IdCarWash = "CW009",
                VehicleLicensePlate = "DEF456",
                IdClient = "987654321",
                IdEmployee = "EMP001",
                WashType = WashType.LaJoya,
                WashStatus = WashStatus.Scheduled,
                CreationDate = DateTime.Now.AddDays(2),
                TotalPrice = 50000,
                Observations = "Lavado La Joya programado"
            },
            new CarWash
            {
                IdCarWash = "CW010",
                VehicleLicensePlate = "MNO345",
                IdClient = "789456123",
                IdEmployee = "EMP002",
                WashType = WashType.Basic,
                WashStatus = WashStatus.Scheduled,
                CreationDate = DateTime.Now.AddDays(1),
                TotalPrice = 15000,
                Observations = "Lavado básico programado"
            }
        };

        // GET: api/CarWash
        [HttpGet]
        public ActionResult<IEnumerable<CarWash>> Get()
        {
            try
            {
                if (carWashs.Count == 0)
                {
                    return NotFound(new { message = "No car washes found" });
                }

                var enrichedCarWashs = carWashs.Select(cw =>
                {
                    var customer = customers.FirstOrDefault(c => c.IdNumber == cw.IdClient);
                    var vehicle = vehicles.FirstOrDefault(v => v.LicensePlate == cw.VehicleLicensePlate);

                    var enrichedCarWash = new CarWash
                    {
                        IdCarWash = cw.IdCarWash,
                        VehicleLicensePlate = cw.VehicleLicensePlate,
                        IdClient = cw.IdClient,
                        IdEmployee = cw.IdEmployee,
                        WashType = cw.WashType,
                        BasePrice = cw.BasePrice,
                        PricetoAgree = cw.PricetoAgree,
                        IVA = cw.IVA,
                        TotalPrice = cw.TotalPrice,
                        WashStatus = cw.WashStatus,
                        CreationDate = cw.CreationDate,
                        Observations = cw.Observations,
                        Customer = customer,
                        Vehicle = vehicle
                    };

                    enrichedCarWash.CalculatePrices();
                    return enrichedCarWash;
                }).ToList();

                return Ok(enrichedCarWashs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving car washes", error = ex.Message });
            }
        }

        // GET: api/CarWash/search?searchTerm=value
        [HttpGet("search")]
        public ActionResult<IEnumerable<CarWash>> GetWithSearch(string searchTerm)
        {
            try
            {
                var filteredCarWashs = carWashs;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filteredCarWashs = carWashs.Where(cw =>
                        cw.IdCarWash.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        cw.VehicleLicensePlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        cw.IdClient.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        cw.IdEmployee.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        cw.WashType.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        cw.WashStatus.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        cw.BasePrice.ToString().Contains(searchTerm) ||
                        cw.TotalPrice.ToString().Contains(searchTerm) ||
                        cw.CreationDate.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                        (cw.Observations?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                var enrichedCarWashs = filteredCarWashs.Select(cw =>
                {
                    var customer = customers.FirstOrDefault(c => c.IdNumber == cw.IdClient);
                    var vehicle = vehicles.FirstOrDefault(v => v.LicensePlate == cw.VehicleLicensePlate);

                    var enrichedCarWash = new CarWash
                    {
                        IdCarWash = cw.IdCarWash,
                        VehicleLicensePlate = cw.VehicleLicensePlate,
                        IdClient = cw.IdClient,
                        IdEmployee = cw.IdEmployee,
                        WashType = cw.WashType,
                        BasePrice = cw.BasePrice,
                        PricetoAgree = cw.PricetoAgree,
                        IVA = cw.IVA,
                        TotalPrice = cw.TotalPrice,
                        WashStatus = cw.WashStatus,
                        CreationDate = cw.CreationDate,
                        Observations = cw.Observations,
                        Customer = customer,
                        Vehicle = vehicle
                    };

                    enrichedCarWash.CalculatePrices();
                    return enrichedCarWash;
                }).ToList();

                return Ok(enrichedCarWashs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching car washes", error = ex.Message });
            }
        }

        // GET: api/CarWash/{id}
        [HttpGet("{id}")]
        public ActionResult<CarWash> Get(string id)
        {
            try
            {
                var carWash = GetCarWashById(id);
                if (carWash == null)
                {
                    return NotFound(new { message = $"Car wash with ID '{id}' not found" });
                }

                var customer = customers.FirstOrDefault(c => c.IdNumber == carWash.IdClient);
                var vehicle = vehicles.FirstOrDefault(v => v.LicensePlate == carWash.VehicleLicensePlate);

                carWash.Customer = customer;
                carWash.Vehicle = vehicle;
                carWash.CalculatePrices();

                return Ok(carWash);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving car wash", error = ex.Message });
            }
        }

        // GET: api/CarWash/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public ActionResult<IEnumerable<CarWash>> GetByCustomer(string customerId)
        {
            try
            {
                var customerCarWashs = carWashs.Where(cw => cw.IdClient == customerId).ToList();

                var customer = customers.FirstOrDefault(c => c.IdNumber == customerId);
                var enrichedCarWashs = customerCarWashs.Select(cw =>
                {
                    var vehicle = vehicles.FirstOrDefault(v => v.LicensePlate == cw.VehicleLicensePlate);
                    cw.Customer = customer;
                    cw.Vehicle = vehicle;
                    cw.CalculatePrices();
                    return cw;
                }).ToList();

                return Ok(enrichedCarWashs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving car washes by customer", error = ex.Message });
            }
        }

        // GET: api/CarWash/vehicle/{licensePlate}
        [HttpGet("vehicle/{licensePlate}")]
        public ActionResult<IEnumerable<CarWash>> GetByVehicle(string licensePlate)
        {
            try
            {
                var vehicleCarWashs = carWashs.Where(cw => cw.VehicleLicensePlate == licensePlate).ToList();

                var vehicle = vehicles.FirstOrDefault(v => v.LicensePlate == licensePlate);
                var enrichedCarWashs = vehicleCarWashs.Select(cw =>
                {
                    var customer = customers.FirstOrDefault(c => c.IdNumber == cw.IdClient);
                    cw.Customer = customer;
                    cw.Vehicle = vehicle;
                    cw.CalculatePrices();
                    return cw;
                }).ToList();

                return Ok(enrichedCarWashs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving car washes by vehicle", error = ex.Message });
            }
        }

        // POST: api/CarWash
        [HttpPost]
        public ActionResult<CarWash> Post([FromBody] CarWash carWash)
        {
            try
            {
                if (carWash == null)
                {
                    return BadRequest(new { message = "Car wash data is required" });
                }

                var validationErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(carWash.IdCarWash))
                {
                    validationErrors.Add("Car wash ID is required.");
                }
                if (string.IsNullOrWhiteSpace(carWash.VehicleLicensePlate))
                {
                    validationErrors.Add("Vehicle license plate is required.");
                }
                if (string.IsNullOrWhiteSpace(carWash.IdClient))
                {
                    validationErrors.Add("Client ID is required.");
                }
                if (string.IsNullOrWhiteSpace(carWash.IdEmployee))
                {
                    validationErrors.Add("Employee ID is required.");
                }

                // Validate La Joya wash type
                if (carWash.WashType == WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    validationErrors.Add("You must specify a price for the 'La Joya' wash.");
                }

                // Check if car wash with same ID already exists
                var existingCarWash = GetCarWashById(carWash.IdCarWash);
                if (existingCarWash != null)
                {
                    validationErrors.Add("A car wash with that ID already exists.");
                }

                // Validate that customer exists
                var customer = customers.FirstOrDefault(c => c.IdNumber == carWash.IdClient);
                if (customer == null)
                {
                    validationErrors.Add("The selected customer does not exist.");
                }

                // Validate that vehicle exists
                var vehicle = vehicles.FirstOrDefault(v => v.LicensePlate == carWash.VehicleLicensePlate);
                if (vehicle == null)
                {
                    validationErrors.Add("The selected vehicle does not exist.");
                }

                // Validate that the vehicle belongs to the customer
                if (customer != null && vehicle != null && vehicle.CustomerId != customer.IdNumber)
                {
                    validationErrors.Add("The selected vehicle does not belong to the selected customer.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                // Set creation date if not provided
                if (carWash.CreationDate == default(DateTime))
                {
                    carWash.CreationDate = DateTime.Now;
                }

                // Calculate prices
                carWash.CalculatePrices();

                // Set navigation properties
                carWash.Customer = customer;
                carWash.Vehicle = vehicle;

                carWashs.Add(carWash);
                return CreatedAtAction(nameof(Get), new { id = carWash.IdCarWash }, carWash);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating car wash", error = ex.Message });
            }
        }

        // PUT: api/CarWash/{id}
        [HttpPut("{id}")]
        public ActionResult<CarWash> Put(string id, [FromBody] CarWash carWash)
        {
            try
            {
                if (carWash == null)
                {
                    return BadRequest(new { message = "Car wash data is required" });
                }

                var existingCarWash = GetCarWashById(id);
                if (existingCarWash == null)
                {
                    return NotFound(new { message = $"Car wash with ID '{id}' not found" });
                }

                var validationErrors = new List<string>();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(carWash.VehicleLicensePlate))
                {
                    validationErrors.Add("Vehicle license plate is required.");
                }
                if (string.IsNullOrWhiteSpace(carWash.IdClient))
                {
                    validationErrors.Add("Client ID is required.");
                }
                if (string.IsNullOrWhiteSpace(carWash.IdEmployee))
                {
                    validationErrors.Add("Employee ID is required.");
                }

                // Validate La Joya wash type
                if (carWash.WashType == WashType.LaJoya && (!carWash.PricetoAgree.HasValue || carWash.PricetoAgree <= 0))
                {
                    validationErrors.Add("You must specify a price for the 'La Joya' wash.");
                }

                // Validate that customer exists
                var customer = customers.FirstOrDefault(c => c.IdNumber == carWash.IdClient);
                if (customer == null)
                {
                    validationErrors.Add("The selected customer does not exist.");
                }

                // Validate that vehicle exists
                var vehicle = vehicles.FirstOrDefault(v => v.LicensePlate == carWash.VehicleLicensePlate);
                if (vehicle == null)
                {
                    validationErrors.Add("The selected vehicle does not exist.");
                }

                // Validate that the vehicle belongs to the customer
                if (customer != null && vehicle != null && vehicle.CustomerId != customer.IdNumber)
                {
                    validationErrors.Add("The selected vehicle does not belong to the selected customer.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                // Ensure ID doesn't change and preserve creation date
                carWash.IdCarWash = id;
                carWash.CreationDate = existingCarWash.CreationDate;

                // Calculate prices
                carWash.CalculatePrices();

                // Set navigation properties
                carWash.Customer = customer;
                carWash.Vehicle = vehicle;

                bool success = UpdateCarWash(carWash);

                if (success)
                {
                    return Ok(carWash);
                }
                else
                {
                    return StatusCode(500, new { message = "Could not update car wash" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating car wash", error = ex.Message });
            }
        }

        // DELETE: api/CarWash/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            try
            {
                var carWash = GetCarWashById(id);
                if (carWash == null)
                {
                    return NotFound(new { message = $"Car wash with ID '{id}' not found" });
                }

                bool success = DeleteCarWash(id);

                if (success)
                {
                    return Ok(new { message = $"Car wash with ID '{id}' deleted successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Could not delete car wash" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting car wash", error = ex.Message });
            }
        }

        #region Private Helper Methods

        private CarWash GetCarWashById(string id)
        {
            return carWashs.FirstOrDefault(c => c.IdCarWash == id);
        }

        private bool UpdateCarWash(CarWash updatedCarWash)
        {
            try
            {
                for (int i = 0; i < carWashs.Count; i++)
                {
                    if (carWashs[i].IdCarWash == updatedCarWash.IdCarWash)
                    {
                        carWashs[i] = updatedCarWash;
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

        private bool DeleteCarWash(string id)
        {
            try
            {
                var carWashToRemove = GetCarWashById(id);
                if (carWashToRemove != null)
                {
                    carWashs.Remove(carWashToRemove);
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