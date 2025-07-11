using dotnet_mvc_car_wash.Models;
using Newtonsoft.Json;
using System.Text;

namespace dotnet_mvc_car_wash.Services
{
    public class ServiceCustomer : IServiceCustomer
    {
        private readonly HttpClient httpClient;
        public ServiceCustomer(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // GET all customers
        public async Task<List<Customer>> Get()
        {
            List<Customer> list = new List<Customer>();
            try
            {
                var response = await httpClient.GetAsync("api/Customer");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<Customer>>(json);
                    if (resultado != null)
                    {
                        list.AddRange(resultado);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error loading customers";
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

        // GET customer by ID
        public async Task<Customer?> GetById(string id)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Customer/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var customer = JsonConvert.DeserializeObject<Customer>(json);
                    return customer;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Customer not found";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error getting customer by ID: {ex.Message}", ex);
            }
        }

        // POST - Create new customer
        public async Task<bool> Save(Customer customer)
        {
            try
            {
                var json = JsonConvert.SerializeObject(customer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Customer", content);

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

                    string errorMessage = errorResponse?.message ?? "Could not create customer";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error saving customer: {ex.Message}", ex);
            }
        }

        // PUT - Update existing customer
        public async Task<bool> Update(Customer customer)
        {
            try
            {
                var json = JsonConvert.SerializeObject(customer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"api/Customer/{customer.IdNumber}", content);

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

                    string errorMessage = errorResponse?.message ?? "Could not update customer";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error updating customer: {ex.Message}", ex);
            }
        }

        // DELETE customer by ID
        public async Task<bool> Delete(string id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/Customer/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Could not delete customer";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error deleting customer: {ex.Message}", ex);
            }
        }

        // GET customers with search filter
        public async Task<List<Customer>> GetWithSearch(string searchTerm)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Customer/search?searchTerm={Uri.EscapeDataString(searchTerm ?? "")}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<Customer>>(json);
                    return resultado ?? new List<Customer>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error searching customers";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error searching customers: {ex.Message}", ex);
            }
        }
    }
}