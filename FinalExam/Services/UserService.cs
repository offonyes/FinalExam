using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FinalExam.Enums;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.User;
using FinalExam.Models.Entities;
using System.Security.Cryptography;
using System.Text;

namespace FinalExam.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<int>> CreateAsync(UserCreateDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.UserName.ToLower() == dto.UserName.ToLower()))
                return new ServiceResponse<int> { Success = false, Message = "User already exists" };

            CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

            var roles = await _context.Roles
                .Where(r => dto.RoleIds.Contains(r.Id))
                .ToListAsync();

            var user = new User
            {
                UserName = dto.UserName,
                PasswordHash = hash,
                PasswordSalt = salt,
                Roles = roles
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<int> { Data = user.Id };
        }

        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return new ServiceResponse<string> { Success = false, Message = "User not found" };

            user.Status = Status.Deleted;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "User deleted successfully" };
        }

        public async Task<ServiceResponse<List<UserDTO>>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Roles)
                .Where(u => !u.Roles.Any(r => r.Name == "Admin"))
                .ToListAsync();

            var result = _mapper.Map<List<UserDTO>>(users);
            return new ServiceResponse<List<UserDTO>> { Data = result };
        }

        public async Task<ServiceResponse<UserDTO>> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return new ServiceResponse<UserDTO> { Success = false, Message = "User not found" };

            return new ServiceResponse<UserDTO> { Data = _mapper.Map<UserDTO>(user) };
        }

        public async Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == dto.UserName.ToLower());

            if (user == null)
                return new ServiceResponse<string> { Success = false, Message = "User not found" };

            var roles = await _context.Roles
                .Where(r => dto.RoleIds.Contains(r.Id))
                .ToListAsync();

            user.Roles = roles;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "User updated successfully" };
        }

        #region Private Methods

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        #endregion
    }
}
