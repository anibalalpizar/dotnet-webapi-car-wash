using dotnet_webapi_car_wash.Models;
using dotnet_webapi_car_wash.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_car_wash.Controllers
{
    [Route("api/CarWash")]
    [ApiController]
    public class CarWashController : ControllerBase
    {
        // Static list to store car washes in memory
        private static List<CarWash> carWashs = new List<CarWash>();

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
                return Ok(carWashs);
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
                    filteredCarWashs = carWashs.Where(l =>
                        l.IdCarWash.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        l.VehicleLicensePlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        l.IdClient.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        l.IdEmployee.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        l.WashType.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        l.WashStatus.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        l.BasePrice.ToString().Contains(searchTerm) ||
                        l.TotalPrice.ToString().Contains(searchTerm) ||
                        l.CreationDate.ToString("dd/MM/yyyy").Contains(searchTerm) ||
                        (l.Observations?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }

                return Ok(filteredCarWashs);
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
                return Ok(carWash);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving car wash", error = ex.Message });
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

                // Validate required fields
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

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                // Ensure ID doesn't change and preserve creation date
                carWash.IdCarWash = id;
                carWash.CreationDate = existingCarWash.CreationDate;

                // Calculate prices
                carWash.CalculatePrices();

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