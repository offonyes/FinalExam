using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.Role;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ServiceResponse<List<RoleDTO>>> GetAllAsync()
        {
            return await _roleService.GetAllAsync();
        }

        [HttpPost]
        public async Task<ServiceResponse<string>> CreateAsync(RoleCreateDTO dto)
        {
            return await _roleService.CreateAsync(dto);
        }

        [HttpGet("{id}")]
        public async Task<ServiceResponse<RoleDTO>> GetByIdAsync(int id)
        {
            return await _roleService.GetByIdAsync(id);
        }

        [HttpPut]
        public async Task<ServiceResponse<string>> UpdateAsync(RoleUpdateDTO dto)
        {
            return await _roleService.UpdateAsync(dto);
        }

        [HttpDelete]
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            return await _roleService.DeleteAsync(id);
        }
    }
}
