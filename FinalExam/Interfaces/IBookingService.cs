using FinalExam.Models;
using FinalExam.Models.DTO_s.Booking;
using FinalExam.Models.DTO_s.Room;
using System.Security.Claims;

namespace FinalExam.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResponse<string>> CreateAsync(BookingCreateDTO dto, ClaimsPrincipal user);
        Task<ServiceResponse<string>> CancelAsync(int id, ClaimsPrincipal user);
        Task<ServiceResponse<string>> UpdateAsync(int id, BookingUpdateDTO dto, ClaimsPrincipal user);
        Task<ServiceResponse<List<BookingDTO>>> GetMyBookingsAsync(ClaimsPrincipal user);
        Task<ServiceResponse<BookingDTO>> GetMyBookingByIdAsync(int id, ClaimsPrincipal user);
        Task<ServiceResponse<List<RoomDTO>>> GetAvailableRoomsAsync(DateTime from, DateTime to);

        Task<ServiceResponse<string>> UpdateStatusAsync(int id, BookingUpdateStatusDTO dto);
        Task<ServiceResponse<List<BookingDTO>>> GetAllAsync();
        Task<ServiceResponse<BookingDTO>> GetByIdAsync(int id); 

    }
}