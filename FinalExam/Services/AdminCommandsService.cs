using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinalExam.Models;
using FinalExam.Models.DTO_s;
using FinalExam.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinalExam.Models.DTO_s.City;
using FinalExam.Models.DTO_s.Hotel;
using FinalExam.Models.DTO_s.Room;
using FinalExam.Models.DTO_s.RoomPhoto;
using FinalExam.Models.DTO_s.RoomType;
using AutoMapper;

namespace FinalExam.Services
{
    public class AdminCommandsService 
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AdminCommandsService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<int>> RegisterAdmin(UserRegisterDTO dto)
        {
            var response = new ServiceResponse<int>();

            if (await UserExists(dto.UserName))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }

            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (userRole == null)
            {
                userRole = new Role { Name = "Admin" };
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

        public async Task<ServiceResponse<string>> SeedInitialDataAsync()
        {
            if (_context.Cities.Any() || _context.Hotels.Any() || _context.Rooms.Any())
            {
                return new ServiceResponse<string> { Success = false, Message = "Data already seeded." };
            }

            var random = new Random();

            var cityDtos = Enumerable.Range(1, 5)
                .Select(i => new CityCreateDTO { Name = $"City {i}" }).ToList();

            var cities = _mapper.Map<List<City>>(cityDtos);
            await _context.Cities.AddRangeAsync(cities);
            await _context.SaveChangesAsync();

            var roomTypeDtos = new List<RoomTypeCreateDTO>
            {
                new() { Name = "Standard" },
                new() { Name = "Deluxe" },
                new() { Name = "Suite" }
            };
            var roomTypes = _mapper.Map<List<RoomType>>(roomTypeDtos);
            await _context.RoomTypes.AddRangeAsync(roomTypes);
            await _context.SaveChangesAsync();

            var hotelDtos = new List<HotelCreateDTO>
            {
                new() { Name = "Hotel A", CityId = cities[0].Id, PhotoUrl = "https://example.com/hotel-a.jpg" },
                new() { Name = "Hotel B", CityId = cities[1].Id, PhotoUrl = "https://example.com/hotel-b.jpg" },
                new() { Name = "Hotel C", CityId = cities[2].Id, PhotoUrl = "https://example.com/hotel-c.jpg" },
                new() { Name = "Hotel D", CityId = cities[3].Id, PhotoUrl = "https://example.com/hotel-d.jpg" },
                new() { Name = "Hotel E", CityId = cities[4].Id, PhotoUrl = "https://example.com/hotel-e.jpg" }
            };
            var hotels = _mapper.Map<List<Hotel>>(hotelDtos);
            await _context.Hotels.AddRangeAsync(hotels);
            await _context.SaveChangesAsync();

            var roomList = new List<Room>();
            foreach (var hotel in hotels)
            {
                for (int i = 1; i <= 5; i++)
                {
                    var roomDto = new RoomCreateDTO
                    {
                        HotelId = hotel.Id,
                        RoomNumber = (100 + i),
                        UnitPrice = random.Next(50, 200),
                        RoomTypeId = roomTypes[random.Next(roomTypes.Count)].Id,
                        RoomPhotos = new List<RoomPhotoCreateDTO>
                                    {
                                        new() { PhotoUrl = $"https://example.com/room{hotel.Id}_{i}_1.jpg" },
                                        new() { PhotoUrl = $"https://example.com/room{hotel.Id}_{i}_2.jpg" }
                                    }
                    };

                    var room = _mapper.Map<Room>(roomDto);
                    roomList.Add(room);
                }
            }

            await _context.Rooms.AddRangeAsync(roomList);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "Initial data seeded successfully." };
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

        #endregion
    }
}
