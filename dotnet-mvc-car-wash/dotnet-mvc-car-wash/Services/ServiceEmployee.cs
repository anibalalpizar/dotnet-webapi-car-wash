using dotnet_mvc_car_wash.Models;
using dotnet_mvc_car_wash.Services;
using Newtonsoft.Json;
using System.Text;

namespace dotnet_mvc_car_wash.Services
{
    public class ServiceEmployee : IServiceEmployee
    {
        private readonly HttpClient httpClient;
        public ServiceEmployee(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // GET all employees
        public async Task<List<Employee>> Get()
        {
            List<Employee> list = new List<Employee>();
            try
            {
                var response = await httpClient.GetAsync("api/Employee");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<Employee>>(json);
                    if (resultado != null)
                    {
                        list.AddRange(resultado);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error loading employees";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error connecting to API: {ex.Message}", ex);
            }
            return list;
        }

        // GET employee by ID
        public async Task<Employee?> GetById(string id)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Employee/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var employee = JsonConvert.DeserializeObject<Employee>(json);
                    return employee;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Employee not found";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error getting employee by ID: {ex.Message}", ex);
            }
        }

        // POST - Create new employee
        public async Task<bool> Save(Employee employee)
        {
            try
            {
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Employee", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    // Manejar errores de validación específicos
                    if (errorResponse?.errors != null)
                    {
                        var validationErrors = new List<string>();
                        foreach (var error in errorResponse.errors)
                        {
                            validationErrors.Add(error.ToString());
                        }
                        throw new Exception(string.Join("; ", validationErrors));
                    }

                    string errorMessage = errorResponse?.message ?? "Could not create employee";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error saving employee: {ex.Message}", ex);
            }
        }

        // PUT - Update existing employee
        public async Task<bool> Update(Employee employee)
        {
            try
            {
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"api/Employee/{employee.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    // Manejar errores de validación específicos
                    if (errorResponse?.errors != null)
                    {
                        var validationErrors = new List<string>();
                        foreach (var error in errorResponse.errors)
                        {
                            validationErrors.Add(error.ToString());
                        }
                        throw new Exception(string.Join("; ", validationErrors));
                    }

                    string errorMessage = errorResponse?.message ?? "Could not update employee";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error updating employee: {ex.Message}", ex);
            }
        }

        // DELETE employee by ID
        public async Task<bool> Delete(string id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/Employee/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Could not delete employee";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error deleting employee: {ex.Message}", ex);
            }
        }
    }
}