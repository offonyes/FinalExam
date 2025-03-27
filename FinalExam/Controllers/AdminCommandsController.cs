using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinalExam.Models.DTO_s;
using FinalExam.Services;
using System.Threading.Tasks;
using FinalExam.Models;

namespace FinalExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCommandsController : ControllerBase
    {
        private readonly AdminCommandsService _adminCommandsService;

        public AdminCommandsController(AdminCommandsService adminCommandsService)
        {
            _adminCommandsService = adminCommandsService;
        }

        [HttpPost("RegisterAdminUser")]
        public async Task<ServiceResponse<int>> RegisterAdmin(UserRegisterDTO dto)
        {
            return await _adminCommandsService.RegisterAdmin(dto);
        }

        [HttpPost("seed-data")]
        public async Task<ActionResult<ServiceResponse<string>>> SeedInitialData()
        {
            var result = await _adminCommandsService.SeedInitialDataAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
