using System.ComponentModel.DataAnnotations;

namespace FinalExam.Models.DTO_s.Booking
{
    public class BookingUpdateDTO
    {
        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }
    }
}
