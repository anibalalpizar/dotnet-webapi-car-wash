using dotnet_mvc_car_wash.Models;
using Newtonsoft.Json;
using System.Text;

namespace dotnet_mvc_car_wash.Services
{
    public class ServiceVehicle : IServiceVehicle
    {
        private readonly HttpClient httpClient;
        public ServiceVehicle(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // GET all vehicles
        public async Task<List<Vehicle>> Get(string searchTerm = null)
        {
            List<Vehicle> list = new List<Vehicle>();
            try
            {
                string endpoint = "api/Vehicle";
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    endpoint += $"?searchTerm={Uri.EscapeDataString(searchTerm)}";
                }

                var response = await httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<Vehicle>>(json);
                    if (resultado != null)
                    {
                        list.AddRange(resultado);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error loading vehicles";
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

        // GET vehicle by ID (license plate)
        public async Task<Vehicle?> GetById(string id)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Vehicle/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var vehicle = JsonConvert.DeserializeObject<Vehicle>(json);
                    return vehicle;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Vehicle not found";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error getting vehicle by ID: {ex.Message}", ex);
            }
        }

        // POST - Create new vehicle
        public async Task<bool> Save(Vehicle vehicle)
        {
            try
            {
                var json = JsonConvert.SerializeObject(vehicle);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Vehicle", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    // Handle specific validation errors
                    if (errorResponse?.errors != null)
                    {
                        var validationErrors = new List<string>();
                        foreach (var error in errorResponse.errors)
                        {
                            validationErrors.Add(error.ToString());
                        }
                        throw new Exception(string.Join("; ", validationErrors));
                    }

                    string errorMessage = errorResponse?.message ?? "Could not create vehicle";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error saving vehicle: {ex.Message}", ex);
            }
        }

        // PUT - Update existing vehicle
        public async Task<bool> Update(Vehicle vehicle)
        {
            try
            {
                var json = JsonConvert.SerializeObject(vehicle);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"api/Vehicle/{vehicle.LicensePlate}", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    // Handle specific validation errors
                    if (errorResponse?.errors != null)
                    {
                        var validationErrors = new List<string>();
                        foreach (var error in errorResponse.errors)
                        {
                            validationErrors.Add(error.ToString());
                        }
                        throw new Exception(string.Join("; ", validationErrors));
                    }

                    string errorMessage = errorResponse?.message ?? "Could not update vehicle";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error updating vehicle: {ex.Message}", ex);
            }
        }

        // DELETE vehicle by ID (license plate)
        public async Task<bool> Delete(string id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/Vehicle/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Could not delete vehicle";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error deleting vehicle: {ex.Message}", ex);
            }
        }

        // GET vehicles with search filter
        public async Task<List<Vehicle>> GetWithSearch(string searchTerm)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Vehicle/search?searchTerm={Uri.EscapeDataString(searchTerm ?? "")}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<Vehicle>>(json);
                    return resultado ?? new List<Vehicle>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error searching vehicles";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error searching vehicles: {ex.Message}", ex);
            }
        }
    }
}