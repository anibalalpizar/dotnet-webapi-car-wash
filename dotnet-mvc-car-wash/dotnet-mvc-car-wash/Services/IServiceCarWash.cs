using dotnet_mvc_car_wash.Models;

namespace dotnet_mvc_car_wash.Services
{
    public interface IServiceCarWash
    {
        Task<List<CarWash>> Get();
        Task<CarWash?> GetById(string id);
        Task<bool> Save(CarWash carWash);
        Task<bool> Update(CarWash carWash);
        Task<bool> Delete(string id);
        Task<List<CarWash>> GetWithSearch(string searchTerm);
    }
}