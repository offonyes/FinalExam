using FinalExam.Interfaces;
using FinalExam.Models;
using FinalExam.Models.DTO_s.Room;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Get all Rooms
        /// </summary>
        [HttpGet]
        public async Task<ServiceResponse<List<RoomDTO>>> GetAllAsync()
        {
            return await _roomService.GetAllAsync();
        }
        [HttpPost]
        public async Task<ServiceResponse<string>> CreateAsync(RoomCreateDTO dto)
        {
            return await _roomService.CreateAsync(dto);
        }
        [HttpGet("{id}")]
        public async Task<ServiceResponse<RoomDTO>> GetByIdAsync(int id)
        {
            return await _roomService.GetByIdAsync(id);
        }
        [HttpPut("{id}")]
        public async Task<ServiceResponse<string>> UpdateAsync(int id, RoomUpdateDTO dto)
        {
            return await _roomService.UpdateAsync(id, dto);
        }
        [HttpDelete("{id}")]
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            return await _roomService.DeleteAsync(id);
        }
    }
}
