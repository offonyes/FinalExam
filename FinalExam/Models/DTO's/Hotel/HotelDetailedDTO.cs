using FinalExam.Models.DTO_s.City;
using FinalExam.Models.DTO_s.Room;

namespace FinalExam.Models.DTO_s.Hotel
{
    public class HotelDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PhotoUrl { get; set; }

        public CityDTO City { get; set; }
        public List<RoomDTO> Rooms { get; set; }
    }
}
