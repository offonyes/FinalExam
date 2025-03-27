using FinalExam.Models;
using FinalExam.Models.DTO_s.Room;

namespace FinalExam.Interfaces
{
    public interface IRoomService
    {
        Task<ServiceResponse<List<RoomDTO>>> GetAllAsync();
        Task<ServiceResponse<RoomDTO>> GetByIdAsync(int id);
        Task<ServiceResponse<string>> CreateAsync(RoomCreateDTO dto);
        Task<ServiceResponse<string>> UpdateAsync(int id, RoomUpdateDTO dto);
        Task<ServiceResponse<string>> DeleteAsync(int id);
    }
}
