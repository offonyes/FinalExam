using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.Role;
using FinalExam.Models.Entities;

namespace FinalExam.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public RoleService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> CreateAsync(RoleCreateDTO dto)
        {
            var mappedRole = _mapper.Map<Role>(dto);

            await _context.Roles.AddAsync(mappedRole);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "Role added successfully" };
        }
        public async Task<ServiceResponse<List<RoleDTO>>> GetAllAsync()
        {
            var roles = await _context.Roles.ToListAsync();

            return new ServiceResponse<List<RoleDTO>>() { Data = roles.Select(x => _mapper.Map<RoleDTO>(x)).ToList() };
        }
        public async Task<ServiceResponse<RoleDTO>> GetByIdAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return new ServiceResponse<RoleDTO> { Success = false, Message = "Role not found" };
            }

            return new ServiceResponse<RoleDTO> { Data = _mapper.Map<RoleDTO>(role) };
        }
        public async Task<ServiceResponse<string>> UpdateAsync(RoleUpdateDTO dto)
        {
            var role = await _context.Roles.FindAsync(dto.Id);

            if (role == null)
            {
                return new ServiceResponse<string> { Success = false, Message = "Role not found" };
            }

            _mapper.Map(dto, role);
            role.LastModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "Role updated successfully" };
        }
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return new ServiceResponse<string> { Success = false, Message = "Role not found" };
            }

            if (role.Users != null && role.Users.Any())
            {
                return new ServiceResponse<string> { Success = false, Message = "Role is assigned to users. Cannot delete." };
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "Role deleted successfully" };
        }
    }
}
