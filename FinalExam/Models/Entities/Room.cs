using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalExam.Models.Entities
{
    public class Room : BaseClass
    {
        public int Id { get; set; } 

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        [MaxLength(10)]
        [Required]
        public int RoomNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public bool IsFavorite { get; set; } = false;


        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }
        public List<RoomPhoto> RoomPhotos { get; set; } = new List<RoomPhoto>();
    }
}
