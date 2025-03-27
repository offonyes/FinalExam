namespace FinalExam.Models
{
    public class BaseClass
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }
        public int? CreatorId { get; set; }
        public int? ModifierId { get; set; }
    }
}
