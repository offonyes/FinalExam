using System.ComponentModel.DataAnnotations;

namespace FinalExam.Models.Entities
{
    public class Role : BaseClass
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}
