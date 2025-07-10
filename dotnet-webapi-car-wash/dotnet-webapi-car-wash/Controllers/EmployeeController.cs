using dotnet_webapi_car_wash.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dotnet_webapi_car_wash.Controllers
{
    [Route("api/Employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        // Static list to store employees in memory
        private static List<Employee> employees = new List<Employee>
        {
            new Employee
            {
                Id = "EMP001",
                BirthDate = new DateTime(1990, 5, 15),
                HireDate = new DateTime(2020, 3, 10),
                DailySalary = 50000,
                AccumulatedVacationDays = 15,
                TerminationDate = null,
                SeveranceAmount = null
            }
        };

        // GET: api/Employee
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> Get()
        {
            try
            {
                if (employees.Count == 0)
                {
                    return NotFound(new { message = "No employees found" });
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employees", error = ex.Message });
            }
        }

        // GET: api/Employee/{id}
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(string id)
        {
            try
            {
                var employee = GetEmployeeById(id);
                if (employee == null)   
                {
                    return NotFound(new { message = $"Employee with ID '{id}' not found" });
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee", error = ex.Message });
            }
        }

        // POST: api/Employee
        [HttpPost]
        public ActionResult<Employee> Post([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest(new { message = "Employee data is required" });
                }

                var validationErrors = new List<string>();

                // Validate birth date vs hire date
                if (employee.BirthDate >= employee.HireDate)
                {
                    validationErrors.Add("The date of birth must be prior to the date of hiring.");
                }

                // Validate age at hire
                var ageAtHire = employee.HireDate.Year - employee.BirthDate.Year;
                if (employee.BirthDate.Date > employee.HireDate.AddYears(-ageAtHire)) ageAtHire--;

                if (ageAtHire < 18)
                {
                    validationErrors.Add("The employee must be at least 18 years old at the time of hiring.");
                }

                // Validate termination date
                if (employee.TerminationDate.HasValue && employee.TerminationDate.Value <= employee.HireDate)
                {
                    validationErrors.Add("The termination date must be after the hire date.");
                }

                // Validate hire date not in future
                if (employee.HireDate > DateTime.Now)
                {
                    validationErrors.Add("The hiring date cannot be in the future.");
                }

                // Check if employee with same ID already exists
                var existingEmployee = GetEmployeeById(employee.Id);
                if (existingEmployee != null)
                {
                    validationErrors.Add("An employee with that ID already exists.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                employees.Add(employee);
                return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating employee", error = ex.Message });
            }
        }

        // PUT: api/Employee/{id}
        [HttpPut("{id}")]
        public ActionResult<Employee> Put(string id, [FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest(new { message = "Employee data is required" });
                }

                var existingEmployee = GetEmployeeById(id);
                if (existingEmployee == null)
                {
                    return NotFound(new { message = $"Employee with ID '{id}' not found" });
                }

                var validationErrors = new List<string>();

                // Validate birth date vs hire date
                if (employee.BirthDate >= employee.HireDate)
                {
                    validationErrors.Add("The date of birth must be prior to the date of hiring.");
                }

                // Validate age at hire
                var ageAtHire = employee.HireDate.Year - employee.BirthDate.Year;
                if (employee.BirthDate.Date > employee.HireDate.AddYears(-ageAtHire)) ageAtHire--;

                if (ageAtHire < 18)
                {
                    validationErrors.Add("The employee must be at least 18 years old at the time of hiring.");
                }

                // Validate termination date
                if (employee.TerminationDate.HasValue && employee.TerminationDate.Value <= employee.HireDate)
                {
                    validationErrors.Add("The termination date must be after the hire date.");
                }

                // Validate hire date not in future
                if (employee.HireDate > DateTime.Now)
                {
                    validationErrors.Add("The hiring date cannot be in the future.");
                }

                if (validationErrors.Any())
                {
                    return BadRequest(new { message = "Validation errors", errors = validationErrors });
                }

                // Ensure ID doesn't change
                employee.Id = id;
                bool success = UpdateEmployee(employee);

                if (success)
                {
                    return Ok(employee);
                }
                else
                {
                    return StatusCode(500, new { message = "Could not update employee" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating employee", error = ex.Message });
            }
        }

        // DELETE: api/Employee/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            try
            {
                var employee = GetEmployeeById(id);
                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID '{id}' not found" });
                }

                bool success = DeleteEmployee(id);

                if (success)
                {
                    return Ok(new { message = $"Employee with ID '{id}' deleted successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Could not delete employee" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting employee", error = ex.Message });
            }
        }
        #region Private Helper Methods

        private Employee GetEmployeeById(string id)
        {
            Employee employee = null;
            foreach (var emp in employees)
            {
                if (emp.Id == id)
                {
                    employee = emp;
                    break;
                }
            }
            return employee;
        }

        private bool UpdateEmployee(Employee updatedEmployee)
        {
            bool success = false;
            try
            {
                for (int i = 0; i < employees.Count; i++)
                {
                    if (employees[i].Id == updatedEmployee.Id)
                    {
                        employees[i] = updatedEmployee;
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

        private bool DeleteEmployee(string id)
        {
            bool success = false;
            try
            {
                Employee employeeToRemove = GetEmployeeById(id);
                if (employeeToRemove != null)
                {
                    employees.Remove(employeeToRemove);
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
