using FinalExam.Models;
using FinalExam.Models.DTO_s;

namespace FinalExam.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegisterDTO dto);
        Task<ServiceResponse<TokenDTO>> Login(UserLoginDTO dto);
        Task<ServiceResponse<TokenDTO>> RefreshToken(RefreshTokenDTO dto);
    }
}
