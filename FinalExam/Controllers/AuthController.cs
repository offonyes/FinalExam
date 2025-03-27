using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ServiceResponse<int>> Register(UserRegisterDTO userRegisterDTO)
        {
            return await _authService.Register(userRegisterDTO);
        }

        [HttpPost("Login")]
        public async Task<ServiceResponse<TokenDTO>> Login(UserLoginDTO userLoginDTO)
        {
            return await _authService.Login(userLoginDTO);
        }

        [HttpPost("RefreshToken")]
        public async Task<ServiceResponse<TokenDTO>> RefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            return await _authService.RefreshToken(refreshTokenDTO);
        }
    }
}
