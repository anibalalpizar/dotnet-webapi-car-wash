using dotnet_mvc_car_wash.Models;

namespace dotnet_mvc_car_wash.Services
{
    public interface IServiceCustomer
    {
        public Task<List<Customer>> Get();
        public Task<bool> Save(Customer customer);
        public Task<bool> Update(Customer customer);
        public Task<bool> Delete(string id);
        public Task<Customer?> GetById(string id);
    }
}
