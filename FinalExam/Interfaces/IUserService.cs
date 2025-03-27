using FinalExam.Models;
using FinalExam.Models.DTO_s.User;

namespace FinalExam.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<List<UserDTO>>> GetAllAsync();
        Task<ServiceResponse<UserDTO>> GetByIdAsync(int id);
        Task<ServiceResponse<int>> CreateAsync(UserCreateDTO dto);
        Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO dto);
        Task<ServiceResponse<string>> DeleteAsync(int id);
    }
}
