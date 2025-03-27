using AutoMapper;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.Room;
using FinalExam.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinalExam.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoomService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> CreateAsync(RoomCreateDTO dto)
        {
            var response = new ServiceResponse<string>();

            var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == dto.HotelId);
            if (!hotelExists)
            {
                response.Success = false;
                response.Message = "Hotel not found";
                return response;
            }

            var typeExists = await _context.RoomTypes.AnyAsync(r => r.Id == dto.RoomTypeId);
            if (!typeExists)
            {
                response.Success = false;
                response.Message = "Room type not found";
                return response;
            }

            var mappedRoom = _mapper.Map<Room>(dto);

            try
            {
                await _context.Rooms.AddAsync(mappedRoom);
                await _context.SaveChangesAsync();

                response.Data = "Room added successfully";
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("IX_Rooms_HotelId_RoomNumber") == true)
                {
                    response.Success = false;
                    response.Message = "Room number already exists in this hotel.";
                }
                else
                {
                    response.Success = false;
                    response.Message = ex.InnerException?.Message;
                }
            }

            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return new ServiceResponse<string> { Success = false, Message = "Room not found" };
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "Room deleted successfully" };
        }

        public async Task<ServiceResponse<List<RoomDTO>>> GetAllAsync()
        {
            var rooms = await _context.Rooms
                .Include(x => x.RoomPhotos)
                .Include(x => x.RoomType)
                .ToListAsync();

            return new ServiceResponse<List<RoomDTO>>
            {
                Data = rooms.Select(x => _mapper.Map<RoomDTO>(x)).ToList()
            };
        }

        public async Task<ServiceResponse<RoomDTO>> GetByIdAsync(int id)
        {
            var room = await _context.Rooms
                .Include(x => x.RoomPhotos)
                .Include(x => x.RoomType)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (room == null)
            {
                return new ServiceResponse<RoomDTO>
                {
                    Success = false,
                    Message = "Room not found"
                };
            }

            return new ServiceResponse<RoomDTO> { Data = _mapper.Map<RoomDTO>(room) };
        }

        public async Task<ServiceResponse<string>> UpdateAsync(int id, RoomUpdateDTO dto)
        {
            var response = new ServiceResponse<string>();
            var room = await _context.Rooms
                .Include(x => x.RoomPhotos)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (room == null)
            {
                response.Success = false;
                response.Message = "Room not found";
                return response;
            }

            if (dto.RoomTypeId.HasValue)
            {
                var exists = await _context.RoomTypes.AnyAsync(x => x.Id == dto.RoomTypeId.Value);
                if (!exists)
                {
                    response.Success = false;
                    response.Message = "Room type not found";
                    return response;
                }
            }

            _mapper.Map(dto, room);

            if (dto.RoomPhotos != null)
            {
                _context.RoomPhotos.RemoveRange(room.RoomPhotos);

                room.RoomPhotos = dto.RoomPhotos.Select(p => new RoomPhoto
                {
                    PhotoUrl = p.PhotoUrl
                }).ToList();
            }

            try
            {
                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
                response.Data = "Room updated successfully";
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("IX_Rooms_HotelId_RoomNumber") == true)
                {
                    response.Success = false;
                    response.Message = "Room number already exists in this hotel.";
                }
                else
                {
                    response.Success = false;
                    response.Message = ex.InnerException?.Message;
                }
            }

            return response;
        }
    }
}
