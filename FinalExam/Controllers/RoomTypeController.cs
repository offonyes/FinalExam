using FinalExam.Interfaces;
using FinalExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinalExam.Models.DTO_s.RoomType;

namespace FinalExam.Controllers
{
    [Route("api")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomtypeService;

        public RoomTypeController(IRoomTypeService roomtypeService)
        {
            _roomtypeService = roomtypeService;
        }

        [HttpGet("GelAllRoomTypes")]
        public async Task<ServiceResponse<List<RoomTypeDTO>>> GetAllAsync()
        {
            return await _roomtypeService.GetAllAsync();
        }

        [HttpPost("RoomTypes")]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<string>> CreateAsync(RoomTypeCreateDTO dto)
        {
            return await _roomtypeService.CreateAsync(dto);
        }

        [HttpDelete("RoomTypes/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            return await _roomtypeService.DeleteAsync(id);
        }

    }
}
