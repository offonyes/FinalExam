using FinalExam.Enums;
using FinalExam.Models.DTO_s.Room;

namespace FinalExam.Models.DTO_s.Booking
{
    public class BookingDTO
    {
        public int Id { get; set; }

        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; }

        public RoomDTO Room { get; set; }
    }
}
