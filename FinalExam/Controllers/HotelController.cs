using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinalExam.Interfaces;
using FinalExam.Models.DTO_s.Hotel;
using FinalExam.Models;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<ServiceResponse<List<HotelListDTO>>> GetAllAsync(string? cityName = null)
        {
            return await _hotelService.GetAllAsync(cityName);
        }

        [HttpGet("{id}")]
        public async Task<ServiceResponse<HotelDetailDTO>> GetByIdAsync(int id)
        {
            return await _hotelService.GetByIdAsync(id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<int>> CreateAsync(HotelCreateDTO dto)
        {
            return await _hotelService.CreateAsync(dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<string>> UpdateAsync(int id, HotelUpdateDTO dto)
        {
            return await _hotelService.UpdateAsync(id, dto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            return await _hotelService.DeleteAsync(id);
        }
    }
}
