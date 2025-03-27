using AutoMapper;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.City;
using FinalExam.Models.DTO_s.RoomType;
using FinalExam.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinalExam.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public RoomTypeService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> CreateAsync(RoomTypeCreateDTO dto)
        {
            var mappedRoomType = _mapper.Map<RoomType>(dto);

            await _context.RoomTypes.AddAsync(mappedRoomType);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "RoopType added successfully" };
        }

        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null)
            {
                return new ServiceResponse<string> { Success = false, Message = "RoomType not found" };

            }

            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
            return new ServiceResponse<string> { Data = "RoomType deleted successfully" };
        }

        public async Task<ServiceResponse<List<RoomTypeDTO>>> GetAllAsync()
        {
            var roomtypes = await _context.RoomTypes.ToListAsync();
            return new ServiceResponse<List<RoomTypeDTO>>()
            {
                Data = roomtypes.Select(x
                => _mapper.Map<RoomTypeDTO>(x)).ToList()
            };
        }
    }
}