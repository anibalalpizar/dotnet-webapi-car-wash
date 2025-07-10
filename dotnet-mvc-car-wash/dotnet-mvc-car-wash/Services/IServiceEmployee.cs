using dotnet_mvc_car_wash.Models;

namespace dotnet_mvc_car_wash.Services
{
    public interface IServiceEmployee
    {
        public Task<List<Employee>> Get();
        public Task<bool> Save(Employee employee);
        public Task<bool> Update(Employee employee);
        public Task<bool> Delete(string id);
        public Task<Employee?> GetById(string id);
    }
}
