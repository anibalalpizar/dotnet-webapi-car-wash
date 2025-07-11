using dotnet_mvc_car_wash.Models;

namespace dotnet_mvc_car_wash.Services
{
    public interface IServiceVehicle
    {
        public Task<List<Vehicle>> Get();
        public Task<bool> Save(Vehicle vehicle);
        public Task<bool> Update(Vehicle vehicle);
        public Task<bool> Delete(string id);
        public Task<Vehicle?> GetById(string id);
    }
}
