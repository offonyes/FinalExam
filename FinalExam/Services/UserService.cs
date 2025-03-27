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
            var response = new ServiceResponse<int>();

            if (await UserExists(dto.UserName))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }

            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User()
            {
                UserName = dto.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var roles = await _context.Roles.Where(x => dto.RoleIds.Contains(x.Id)).ToListAsync();

            user.Roles = roles;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                user.Status = Status.Deleted;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new ServiceResponse<string>() { Data = "User deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>() { Success = false, Message = "ex.GetFullMessage()" };
            }
        }

        public async Task<ServiceResponse<List<UserDTO>>> GetAllAsync()
        {
            var users = await _context.Users.Include(x => x.Roles).ToListAsync();
            var test = false;
            var notAdminUsers = new List<User>();
            foreach (var user in users)
            {
                test = false;
                var roles = user.Roles;
                foreach (var role in roles)
                {
                    if (role.Name != "Admin")
                    {
                        test = true;
                    }
                }
                if (test)
                    notAdminUsers.Add(user);
            }

            return new ServiceResponse<List<UserDTO>>() { Data = users.Select(x => _mapper.Map<UserDTO>(x)).ToList() };
        }

        public async Task<ServiceResponse<UserDTO>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

                return new ServiceResponse<UserDTO>() { Data = _mapper.Map<UserDTO>(user) };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserDTO>() { Success = false, Message = "ex.GetFullMessage()" };
            }
        }

        public async Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO dto)
        {
            var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.UserName.ToLower() == dto.UserName.ToLower());

            if (user == null)
                return new ServiceResponse<string>() { Success = false, Data = "User not found" };

            var roles = await _context.Roles.Where(x => dto.RoleIds.Contains(x.Id)).ToListAsync();

            user.Roles = roles;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string>() { Data = "User updated successfully" };
        }

        #region PrivateMethods

        private async Task<bool> UserExists(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower()))
                return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        #endregion
    }
}
