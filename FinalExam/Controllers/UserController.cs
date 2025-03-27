using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalExam.Models.DTO_s.User;
using FinalExam.Models;
using FinalExam.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ServiceResponse<int>> CreateAsync(UserCreateDTO dto)
        {
            return await _userService.CreateAsync(dto);
        }

        [HttpPut]
        public async Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO dto)
        {
            return await _userService.UpdateAsync(dto);
        }

        [HttpGet]
        public async Task<ServiceResponse<List<UserDTO>>> GetAllAsync()
        {
            return await _userService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ServiceResponse<UserDTO>> GetByIdAsync(int id)
        {
            return await _userService.GetByIdAsync(id);
        }

        [HttpDelete]
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            return await _userService.DeleteAsync(id);
        }
    }
}
