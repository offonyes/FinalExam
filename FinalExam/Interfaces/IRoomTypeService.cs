using FinalExam.Models.DTO_s.RoomType;
using FinalExam.Models;

namespace FinalExam.Interfaces
{
    public interface IRoomTypeService
    {
        Task<ServiceResponse<List<RoomTypeDTO>>> GetAllAsync();
        Task<ServiceResponse<string>> CreateAsync(RoomTypeCreateDTO dto);
        Task<ServiceResponse<string>> DeleteAsync(int id);
    }
}
