using FinalExam.Models.DTO_s.RoomPhoto;

namespace FinalExam.Models.DTO_s.Room
{
    public class RoomCreateDTO
    {
        public int HotelId { get; set; } 
        public int RoomNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public int RoomTypeId { get; set; }
        public List<RoomPhotoCreateDTO> RoomPhotos { get; set; } = new List<RoomPhotoCreateDTO>();
    }
}
