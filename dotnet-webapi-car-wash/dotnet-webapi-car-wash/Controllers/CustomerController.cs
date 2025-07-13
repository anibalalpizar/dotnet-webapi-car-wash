using dotnet_webapi_car_wash.Models;
using dotnet_webapi_car_wash.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_car_wash.Controllers
{
    [Route("api/Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        // Static list to store customers in memory
        public static List<Customer> customers = new List<Customer>
{
    new Customer
    {
        IdNumber = "123456789",
        FullName = "John Doe",
        Province = "San José",
        Canton = "Central",
        District = "Carmen",
        ExactAddress = "100 metros norte del parque central",
        Phone = "8888-8888",
        WashPreference = WashPreference.Weekly
    },
    new Customer
    {
        IdNumber = "987654321",
        FullName = "María González",
        Province = "Alajuela",
        Canton = "Alajuela",
        District = "Alajuela",
        ExactAddress = "200 metros sur de la iglesia",
        Phone = "7777-7777",
        WashPreference = WashPreference.Monthly
    },
    new Customer
    {
        IdNumber = "456789123",
        FullName = "Carlos Rodríguez",
        Province = "Cartago",
        Canton = "Cartago",
        District = "Oriental",
        ExactAddress = "Del mall 300 metros este",
        Phone = "6666-6666",
        WashPreference = WashPreference.Biweekly
    },
    new Customer
    {
        IdNumber = "321654987",
        FullName = "Ana Jiménez",
        Province = "Heredia",
        Canton = "Heredia",
        District = "Heredia",
        ExactAddress = "Frente al hospital",
        Phone = "5555-5555",
        WashPreference = WashPreference.Other
    },
    new Customer
    {
        IdNumber = "789456123",
        FullName = "Luis Fernández",
        Province = "Puntarenas",
        Canton = "Puntarenas",
        District = "Puntarenas",
        ExactAddress = "Cerca del puerto",
        Phone = "4444-4444",
        WashPreference = WashPreference.Weekly
    }
};

        // GET: api/Customer
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> Get()
        {
            try
            {
                if (customers.Count == 0)
                {
                    return NotFound(new { message = "No customers found" });
                }
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving customers", error = ex.Message });
            }
        }

        // GET: api/Customer/search?searchTerm=value
        [HttpGet("search")]
        public ActionResult<IEnumerable<Customer>> GetWithSearch(string searchTerm)
        {
            try
            {
                var filteredCustomers = customers;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filteredCustomers = customers.Where(c =>
                        c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.Province.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.Canton.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.District.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.ExactAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.WashPreference.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                return Ok(filteredCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching customers", error = ex.Message });
            }
        }

        // GET: api/Customer/{id}
        [HttpGet("{id}")]
        public ActionResult<Customer> Get(string id)
        {
            try
            {
                var customer = GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with ID '{id}' not found" });
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving customer", error = ex.Message });
            }
        }

        // POST: api/Customer
        [HttpPost]
        public ActionResult<Customer> Post([FromBody] Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest(new { message = "Customer data is required" });
                }

                var validationErrors = new List<string>();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(customer.IdNumber))
                {
                    validationErrors.Add("ID Number is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.FullName))
                {
                    validationErrors.Add("Full Name is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.Province))
                {
                    validationErrors.Add("Province is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.Canton))
                {
                    validationErrors.Add("Canton is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.District))
                {
                    validationErrors.Add("District is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.ExactAddress))
                {
                    validationErrors.Add("Exact Address is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.Phone))
                {
                    validationErrors.Add("Phone is required.");
                }

                // Check if customer with same ID already exists
                var existingCustomer = GetCustomerById(customer.IdNumber);
                if (existingCustomer != null)
                {
                    validationErrors.Add("A customer with that ID already exists.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                customers.Add(customer);
                return CreatedAtAction(nameof(Get), new { id = customer.IdNumber }, customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating customer", error = ex.Message });
            }
        }

        // PUT: api/Customer/{id}
        [HttpPut("{id}")]
        public ActionResult<Customer> Put(string id, [FromBody] Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest(new { message = "Customer data is required" });
                }

                var existingCustomer = GetCustomerById(id);
                if (existingCustomer == null)
                {
                    return NotFound(new { message = $"Customer with ID '{id}' not found" });
                }

                var validationErrors = new List<string>();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(customer.FullName))
                {
                    validationErrors.Add("Full Name is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.Province))
                {
                    validationErrors.Add("Province is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.Canton))
                {
                    validationErrors.Add("Canton is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.District))
                {
                    validationErrors.Add("District is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.ExactAddress))
                {
                    validationErrors.Add("Exact Address is required.");
                }

                if (string.IsNullOrWhiteSpace(customer.Phone))
                {
                    validationErrors.Add("Phone is required.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                // Ensure ID doesn't change
                customer.IdNumber = id;
                bool success = UpdateCustomer(customer);

                if (success)
                {
                    return Ok(customer);
                }
                else
                {
                    return StatusCode(500, new { message = "Could not update customer" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating customer", error = ex.Message });
            }
        }

        // DELETE: api/Customer/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            try
            {
                var customer = GetCustomerById(id);
                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with ID '{id}' not found" });
                }

                bool success = DeleteCustomer(id);

                if (success)
                {
                    return Ok(new { message = $"Customer with ID '{id}' deleted successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Could not delete customer" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting customer", error = ex.Message });
            }
        }

        #region Private Helper Methods

        private Customer GetCustomerById(string id)
        {
            Customer customer = null;
            foreach (var cust in customers)
            {
                if (cust.IdNumber == id)
                {
                    customer = cust;
                    break;
                }
            }
            return customer;
        }

        private bool UpdateCustomer(Customer updatedCustomer)
        {
            bool success = false;
            try
            {
                for (int i = 0; i < customers.Count; i++)
                {
                    if (customers[i].IdNumber == updatedCustomer.IdNumber)
                    {
                        customers[i] = updatedCustomer;
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

        private bool DeleteCustomer(string id)
        {
            bool success = false;
            try
            {
                var customerToRemove = GetCustomerById(id);
                if (customerToRemove != null)
                {
                    customers.Remove(customerToRemove);
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