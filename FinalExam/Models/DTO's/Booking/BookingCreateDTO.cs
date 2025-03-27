namespace FinalExam.Models.DTO_s.Booking
{
    public class BookingCreateDTO
    {
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
