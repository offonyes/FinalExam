using System.ComponentModel.DataAnnotations;

namespace FinalExam.Models.Entities
{
    public class RoomType : BaseClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
