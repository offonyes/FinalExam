using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FinalExam.Models;
using FinalExam.Models.DTO_s.City;
using FinalExam.Models.Entities;
using System.Data;
using FinalExam.Interfaces;

namespace FinalExam.Services
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CityService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<CityDTO>>> GetAllAsync()
        {
            var cities = await _context.Cities.ToListAsync();
            return new ServiceResponse<List<CityDTO>>() { Data = cities.Select(x => _mapper.Map<CityDTO>(x)).ToList() };

        }

        public async Task<ServiceResponse<string>> CreateAsync(CityCreateDTO dto)
        {
            var mappedCity = _mapper.Map<City>(dto);

            await _context.Cities.AddAsync(mappedCity);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = "City added successfully" };
        }

        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return new ServiceResponse<string> { Success = false, Message = "City not found" };

            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return new ServiceResponse<string> { Data = "City deleted successfully" };
        }
    }
}
