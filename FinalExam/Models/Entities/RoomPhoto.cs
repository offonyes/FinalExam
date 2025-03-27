using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalExam.Models.Entities
{
    public class RoomPhoto : BaseClass
    {
        [Key]
        public int Id { get; set; } 

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [Required]
        public string PhotoUrl { get; set; }
    }
}
