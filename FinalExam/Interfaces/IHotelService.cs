using FinalExam.Models;
using FinalExam.Models.DTO_s.Hotel;

namespace FinalExam.Interfaces
{
    public interface IHotelService
    {
        Task<ServiceResponse<List<HotelListDTO>>> GetAllAsync(string? cityName = null);
        Task<ServiceResponse<HotelDetailDTO>> GetByIdAsync(int id);
        Task<ServiceResponse<int>> CreateAsync(HotelCreateDTO dto);
        Task<ServiceResponse<string>> UpdateAsync(int id, HotelUpdateDTO dto);
        Task<ServiceResponse<string>> DeleteAsync(int id);
    }
}
