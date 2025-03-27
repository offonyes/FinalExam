using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.Booking;
using FinalExam.Models.DTO_s.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("GetAvailableRooms")]
        public async Task<ServiceResponse<List<RoomDTO>>> GetAvailableRooms([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return await _bookingService.GetAvailableRoomsAsync(from, to);
        }

        /// <summary>
        /// Create a booking (User only)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ServiceResponse<string>> CreateAsync(BookingCreateDTO dto)
        {
            return await _bookingService.CreateAsync(dto, User);
        }

        /// <summary>
        /// Get all my bookings (User only)
        /// </summary>
        [HttpGet("my")]
        [Authorize]
        public async Task<ServiceResponse<List<BookingDTO>>> GetMyBookingsAsync()
        {
            return await _bookingService.GetMyBookingsAsync(User);
        }

        /// <summary>
        /// Get one of my bookings by ID (User only)
        /// </summary>
        [HttpGet("my/{id}")]
        [Authorize]
        public async Task<ServiceResponse<BookingDTO>> GetMyBookingByIdAsync(int id)
        {
            return await _bookingService.GetMyBookingByIdAsync(id, User);
        }

        /// <summary>
        /// Cancel my booking (User only)
        /// </summary>
        [HttpPut("my/cancel/{id}")]
        [Authorize]
        public async Task<ServiceResponse<string>> CancelAsync(int id)
        {
            return await _bookingService.CancelAsync(id, User);
        }

        /// <summary>
        /// Update my booking dates (User only)
        /// </summary>
        [HttpPut("my/{id}")]
        [Authorize]
        public async Task<ServiceResponse<string>> UpdateAsync(int id, BookingUpdateDTO dto)
        {
            return await _bookingService.UpdateAsync(id, dto, User);
        }

        /// <summary>
        /// Get all bookings (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Tags("Admin Bookings")]
        public async Task<ServiceResponse<List<BookingDTO>>> GetAllAsync()
        {
            return await _bookingService.GetAllAsync();
        }

        /// <summary>
        /// Get booking by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [Tags("Admin Bookings")]
        public async Task<ServiceResponse<BookingDTO>> GetByIdAsync(int id)
        {
            return await _bookingService.GetByIdAsync(id);
        }

        /// <summary>
        /// Update booking status (Admin only)
        /// </summary>
        [HttpPatch("status/{id}")]
        [Authorize(Roles = "Admin")]
        [Tags("Admin Bookings")]
        public async Task<ServiceResponse<string>> UpdateStatusAsync(int id, BookingUpdateStatusDTO dto)
        {
            return await _bookingService.UpdateStatusAsync(id, dto);
        }
    }
}
