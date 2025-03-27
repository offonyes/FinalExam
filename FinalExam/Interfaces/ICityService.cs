using FinalExam.Models.DTO_s.City;
using FinalExam.Models;

namespace FinalExam.Interfaces
{
    public interface ICityService
    {
        Task<ServiceResponse<List<CityDTO>>> GetAllAsync();
        Task<ServiceResponse<string>> CreateAsync(CityCreateDTO dto);
        Task<ServiceResponse<string>> DeleteAsync(int id);
    }
}
