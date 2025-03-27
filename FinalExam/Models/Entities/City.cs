using System.ComponentModel.DataAnnotations;

namespace FinalExam.Models.Entities
{
    public class City : BaseClass
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
