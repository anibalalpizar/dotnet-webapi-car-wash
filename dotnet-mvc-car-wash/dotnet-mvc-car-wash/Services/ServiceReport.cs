using dotnet_mvc_car_wash.Models;
using Newtonsoft.Json;

namespace dotnet_mvc_car_wash.Services
{
    public class ServiceReport : IServiceReport
    {
        private readonly HttpClient httpClient;

        public ServiceReport(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // GET clients to contact report
        public async Task<ClientsToContactResponse> GetClientsToContact()
        {
            try
            {
                var response = await httpClient.GetAsync("api/Report/clients-to-contact");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ClientsToContactResponse>(json);
                    return result ?? new ClientsToContactResponse();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error loading clients to contact report";
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
        }

        // GET wash statistics report
        public async Task<WashStatistics> GetWashStatistics()
        {
            try
            {
                var response = await httpClient.GetAsync("api/Report/wash-statistics");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<WashStatistics>(json);
                    return result ?? new WashStatistics();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error loading wash statistics";
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
        }

        // GET customer activity report
        public async Task<CustomerActivity> GetCustomerActivity(string customerId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Report/customer-activity/{customerId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CustomerActivity>(json);
                    return result ?? new CustomerActivity();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    string errorMessage = errorResponse?.message ?? "Error loading customer activity";
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
        }
    }
}
