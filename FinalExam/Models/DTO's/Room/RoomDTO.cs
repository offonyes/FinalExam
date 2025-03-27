    using FinalExam.Models.DTO_s.RoomPhoto;
using FinalExam.Models.DTO_s.RoomType;

namespace FinalExam.Models.DTO_s.Room
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsFavorite { get; set; }
        public string RoomTypeName { get; set; }
        public List<RoomPhotoDTO> RoomPhotos { get; set; }
    }
}
