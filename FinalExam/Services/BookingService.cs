using AutoMapper;
using FinalExam.Enums;
using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.Booking;
using FinalExam.Models.DTO_s.Room;
using FinalExam.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalExam.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<string>> CreateAsync(BookingCreateDTO dto, ClaimsPrincipal user)
        {
            var response = new ServiceResponse<string>();

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == dto.RoomId);
            if (room == null)
            {
                response.Success = false;
                response.Message = "Room not found";
                return response;
            }

            var validation = await ValidateBookingDatesAsync(0, dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
            if (!validation.Success)
                return validation;

            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
            var days = (dto.CheckOutDate - dto.CheckInDate).Days;

            var booking = _mapper.Map<Booking>(dto);
            booking.UserId = userId;
            booking.TotalPrice = days * room.UnitPrice;
            booking.Status = BookingStatus.Pending;

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            response.Data = "Booking created successfully";
            return response;
        }

        public async Task<ServiceResponse<string>> CancelAsync(int id, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return new ServiceResponse<string> { Success = false, Message = "Booking not found or access denied" };

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "Booking cancelled" };
        }
        public async Task<ServiceResponse<string>> UpdateAsync(int id, BookingUpdateDTO dto, ClaimsPrincipal user)
        {
            var response = new ServiceResponse<string>();
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                response.Success = false;
                response.Message = "Booking not found or access denied";
                return response;
            }

            var validation = await ValidateBookingDatesAsync(booking.Id, booking.RoomId, dto.CheckInDate, dto.CheckOutDate);
            if (!validation.Success)
                return validation;

            var days = (dto.CheckOutDate - dto.CheckInDate).Days;

            _mapper.Map(dto, booking);
            booking.TotalPrice = days * booking.Room.UnitPrice;

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            response.Data = "Booking updated successfully";
            return response;
        }

        public async Task<ServiceResponse<BookingDTO>> GetMyBookingByIdAsync(int id, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
            var booking = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomPhotos)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return new ServiceResponse<BookingDTO> { Success = false, Message = "Booking not found" };

            return new ServiceResponse<BookingDTO> { Data = _mapper.Map<BookingDTO>(booking) };
        }

        public async Task<ServiceResponse<List<BookingDTO>>> GetMyBookingsAsync(ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomPhotos)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return new ServiceResponse<List<BookingDTO>>
            {
                Data = _mapper.Map<List<BookingDTO>>(bookings)
            };
        }

        public async Task<ServiceResponse<List<BookingDTO>>> GetAllAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomPhotos)
                .Include(b => b.User)
                .ToListAsync();

            return new ServiceResponse<List<BookingDTO>>
            {
                Data = _mapper.Map<List<BookingDTO>>(bookings)
            };
        }

        public async Task<ServiceResponse<BookingDTO>> GetByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomPhotos)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);


            if (booking == null)
                return new ServiceResponse<BookingDTO> { Success = false, Message = "Booking not found" };

            return new ServiceResponse<BookingDTO> { Data = _mapper.Map<BookingDTO>(booking) };
        }

        public async Task<ServiceResponse<string>> UpdateStatusAsync(int id, BookingUpdateStatusDTO dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return new ServiceResponse<string> { Success = false, Message = "Booking not found" };

            booking.Status = dto.Status;
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "Booking status updated" };
        }

        public async Task<ServiceResponse<List<RoomDTO>>> GetAvailableRoomsAsync(DateTime from, DateTime to)
        {
            var response = new ServiceResponse<List<RoomDTO>>();

            if (from.Date < DateTime.Today || to.Date <= from.Date)
            {
                response.Success = false;
                response.Message = "Invalid date range";
                return response;
            }

            var bookedRoomIds = await _context.Bookings
                .Where(b =>
                    (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) &&
                    b.CheckInDate.Date < to.Date &&
                    b.CheckOutDate.Date > from.Date)
                .Select(b => b.RoomId)
                .ToListAsync();

            var availableRooms = await _context.Rooms
                .Include(r => r.RoomPhotos)
                .Include(r => r.RoomType)
                .Where(r => !bookedRoomIds.Contains(r.Id))
                .ToListAsync();

            response.Data = _mapper.Map<List<RoomDTO>>(availableRooms);
            return response;
        }

        #region Private Methods
        private async Task<ServiceResponse<string>> ValidateBookingDatesAsync(int bookingId, int roomId, DateTime checkIn, DateTime checkOut)
        {
            var response = new ServiceResponse<string>();

            if (checkIn.Date < DateTime.Now.Date.AddDays(2))
            {
                response.Success = false;
                response.Message = "Check-in date must be at least 2 days from today.";
                return response;
            }

            var days = (checkOut - checkIn).Days;
            if (days <= 0)
            {
                response.Success = false;
                response.Message = "Invalid date range: Check-out must be after Check-in.";
                return response;
            }

            var hasConflict = await _context.Bookings.AnyAsync(b =>
                b.Id != bookingId &&
                b.RoomId == roomId &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) &&
                (
                    (checkIn >= b.CheckInDate && checkIn < b.CheckOutDate) ||
                    (checkOut > b.CheckInDate && checkOut <= b.CheckOutDate) ||
                    (checkIn <= b.CheckInDate && checkOut >= b.CheckOutDate)
                ));

            if (hasConflict)
            {
                response.Success = false;
                response.Message = "Room is already booked for the selected dates.";
                return response;
            }

            return response; 
        }

    }
    #endregion
}  