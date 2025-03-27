using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinalExam.Interfaces;
using FinalExam.Models.DTO_s.City;
using FinalExam.Models;

namespace FinalExam.Controllers
{
    [Route("api")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("GetAllCities")]
        public async Task<ServiceResponse<List<CityDTO>>> GetAllAsync()
        {
            return await _cityService.GetAllAsync();
        }

        [HttpPost("Cities")]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<string>> CreateAsync(CityCreateDTO dto)
        {
            return await _cityService.CreateAsync(dto);
        }

        [HttpDelete("Cities/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            return await _cityService.DeleteAsync(id);
        }
    }
}
