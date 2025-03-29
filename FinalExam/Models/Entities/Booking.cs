using FinalExam.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalExam.Models.Entities
{
    public class Booking : BaseClass
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Room")]
        [Required]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }
}
