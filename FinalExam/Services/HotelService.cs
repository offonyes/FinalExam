using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.Entities;
using FinalExam.Models.DTO_s.Hotel;

namespace FinalExam.Services
{
    public class HotelService : IHotelService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HotelService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<HotelListDTO>>> GetAllAsync(string? cityName = null)
        {
            var response = new ServiceResponse<List<HotelListDTO>>();

            var query = _context.Hotels
                .Include(h => h.City)
                .AsQueryable();

            if (!string.IsNullOrEmpty(cityName))
            {
                query = query.Where(h => h.City.Name.ToLower().Contains(cityName.ToLower()));
            }

            var hotels = await query.ToListAsync();

            response.Data = _mapper.Map<List<HotelListDTO>>(hotels);
            return response;
        }

        public async Task<ServiceResponse<HotelDetailDTO>> GetByIdAsync(int id)
        {
            var response = new ServiceResponse<HotelDetailDTO>();

            var hotel = await _context.Hotels
                .Include(h => h.City)
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomPhotos)
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
            {
                response.Success = false;
                response.Message = "Hotel not found";
                return response;
            }

            response.Data = _mapper.Map<HotelDetailDTO>(hotel);
            return response;
        }

        public async Task<ServiceResponse<int>> CreateAsync(HotelCreateDTO dto)
        {
            var response = new ServiceResponse<int>();

            var city = await _context.Cities.FindAsync(dto.CityId);
            if (city == null)
            {
                response.Success = false;
                response.Message = "City not found";
                return response;
            }

            var hotel = _mapper.Map<Hotel>(dto);

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            response.Data = hotel.Id;
            return response;
        }

        public async Task<ServiceResponse<string>> UpdateAsync(int id, HotelUpdateDTO dto)
        {
            var response = new ServiceResponse<string>();
            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
            {
                response.Success = false;
                response.Message = "Hotel not found";
                return response;
            }

            if (dto.CityId.HasValue)
            {
                var cityExists = await _context.Cities.AnyAsync(c => c.Id == dto.CityId.Value);
                if (!cityExists)
                {
                    response.Success = false;
                    response.Message = "City not found";
                    return response;
                }
            }

            _mapper.Map(dto, hotel);

            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();

            response.Data = "Hotel updated successfully";
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            var response = new ServiceResponse<string>();
            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
            {
                response.Success = false;
                response.Message = "Hotel not found";
                return response;
            }

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            response.Data = "Hotel deleted successfully";
            return response;
        }
    }
}
