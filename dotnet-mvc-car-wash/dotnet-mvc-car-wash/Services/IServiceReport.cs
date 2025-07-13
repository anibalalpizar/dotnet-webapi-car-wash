using dotnet_mvc_car_wash.Models;

namespace dotnet_mvc_car_wash.Services
{
    public interface IServiceReport
    {
        Task<ClientsToContactResponse> GetClientsToContact();
        Task<WashStatistics> GetWashStatistics();
        Task<CustomerActivity> GetCustomerActivity(string customerId);
    }
}
