using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s;
using FinalExam.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinalExam.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<int>> Register(UserRegisterDTO dto)
        {
            var response = new ServiceResponse<int>();

            if (await UserExists(dto.UserName))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }

            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole == null)
            {
                userRole = new Role { Name = "User" };
                await _context.Roles.AddAsync(userRole);
                await _context.SaveChangesAsync(); 
            }

            var user = new User()
            {
                UserName = dto.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Roles = new List<Role> { userRole } 
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;
            return response;
        }

        public async Task<ServiceResponse<TokenDTO>> Login(UserLoginDTO dto)
        {
            var response = new ServiceResponse<TokenDTO>();

            var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.UserName.ToLower() == dto.UserName.ToLower());

            if (user != null && user.Status != Enums.Status.Active)
                return new ServiceResponse<TokenDTO> { Success = false, Message = "UserNotActiveError" };

            var roleNames = user?.Roles?.Select(x => x.Name).ToList() ?? new List<string>();

            if (user == null)
            {
                response.Success = false;
                response.Message = "UserNotFound";
                return response;
            }
            else if (!VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Incorrect password";
                return response;
            }
            else
            {
                var result = GenerateTokens(user, dto.StaySignedIn, roleNames);
                response.Data = result;
            }

            if (dto.StaySignedIn)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            return response;
        }

        public async Task<ServiceResponse<TokenDTO>> RefreshToken(RefreshTokenDTO dto)
        {
            var response = new ServiceResponse<TokenDTO>();

            var user = await _context.Users.Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);

            if (user == null || user.RefreshTokenExpirationDate < DateTime.UtcNow)
            {
                response.Success = false;
                response.Message = "Invalid or expired refresh token";
                return response;
            }

            var roleNames = user.Roles.Select(r => r.Name).ToList();

            var newTokens = GenerateTokens(user, dto.StaySignedIn, roleNames);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = newTokens;
            return response;
        }

        #region PrivateMethods

        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private TokenDTO GenerateTokens(User user, bool staySignedIn, List<string>? roleNames)
        {
            string refreshToken = string.Empty;

            if (staySignedIn)
            {
                refreshToken = GenerateRefreshToken(user, roleNames);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpirationDate = DateTime.Now.AddDays(2);
            }

            var accessToken = GenerateAccessToken(user, roleNames);

            return new TokenDTO() { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        private string GenerateAccessToken(User user, List<string>? roleNames)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            if (roleNames != null)
                claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTOptions:Secret").Value ?? string.Empty));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            double.TryParse(_configuration.GetSection("JWTOptions:accessTokenExpirationDays").Value, out double accessTokenExpirationDays);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(accessTokenExpirationDays),
                SigningCredentials = credentials,
                Issuer = _configuration.GetSection("JWTOptions:Issuer").Value,
                Audience = _configuration.GetSection("JWTOptions:Audience").Value
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            SecurityToken token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        private string GenerateRefreshToken(User user, List<string>? roleNames)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            if (roleNames != null)
                claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTOptions:Secret").Value ?? string.Empty));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            double.TryParse(_configuration.GetSection("JWTOptions:refreshTokenExpirationDays").Value, out double refreshTokenExpirationDays);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(refreshTokenExpirationDays),
                SigningCredentials = credentials,
                Issuer = _configuration.GetSection("JWTOptions:Issuer").Value,
                Audience = _configuration.GetSection("JWTOptions:Audience").Value
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(tokenDescriptor);

            user.RefreshToken = handler.WriteToken(token);
            user.RefreshTokenExpirationDate = DateTime.Now.AddDays(refreshTokenExpirationDays);
            return handler.WriteToken(token);
        }

        #endregion
    }
}
