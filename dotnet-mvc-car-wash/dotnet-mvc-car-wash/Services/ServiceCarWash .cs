using dotnet_mvc_car_wash.Models;
using Newtonsoft.Json;
using System.Text;

namespace dotnet_mvc_car_wash.Services
{
    public class ServiceCarWash : IServiceCarWash
    {
        private readonly HttpClient httpClient;
        public ServiceCarWash(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // GET all car washes
        public async Task<List<CarWash>> Get()
        {
            List<CarWash> list = new List<CarWash>();
            try
            {
                var response = await httpClient.GetAsync("api/CarWash");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<CarWash>>(json);
                    if (resultado != null)
                    {
                        list.AddRange(resultado);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error loading car washes";
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

        // GET car wash by ID
        public async Task<CarWash?> GetById(string id)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/CarWash/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var carWash = JsonConvert.DeserializeObject<CarWash>(json);
                    return carWash;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Car wash not found";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error getting car wash by ID: {ex.Message}", ex);
            }
        }

        // POST - Create new car wash
        public async Task<bool> Save(CarWash carWash)
        {
            try
            {
                var json = JsonConvert.SerializeObject(carWash);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/CarWash", content);

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

                    string errorMessage = errorResponse?.message ?? "Could not create car wash";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error saving car wash: {ex.Message}", ex);
            }
        }

        // PUT - Update existing car wash
        public async Task<bool> Update(CarWash carWash)
        {
            try
            {
                var json = JsonConvert.SerializeObject(carWash);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"api/CarWash/{carWash.IdCarWash}", content);

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

                    string errorMessage = errorResponse?.message ?? "Could not update car wash";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error updating car wash: {ex.Message}", ex);
            }
        }

        // DELETE car wash by ID
        public async Task<bool> Delete(string id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/CarWash/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Could not delete car wash";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error deleting car wash: {ex.Message}", ex);
            }
        }

        // GET car washes with search filter
        public async Task<List<CarWash>> GetWithSearch(string searchTerm)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/CarWash/search?searchTerm={Uri.EscapeDataString(searchTerm ?? "")}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<List<CarWash>>(json);
                    return resultado ?? new List<CarWash>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error searching car washes";
                    throw new Exception(errorMessage);
                }
            }
            catch (JsonException)
            {
                throw new Exception("Error processing server response");
            }
            catch (Exception ex) when (!(ex is Exception && ex.Message.Contains("Error")))
            {
                throw new Exception($"Error searching car washes: {ex.Message}", ex);
            }
        }
    }
}