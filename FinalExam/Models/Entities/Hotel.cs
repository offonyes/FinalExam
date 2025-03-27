using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalExam.Models.Entities
{
    public class Hotel : BaseClass
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public City City { get; set; }

        public string? PhotoUrl { get; set; }

        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
