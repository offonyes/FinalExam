using FinalExam.Enums;
using FinalExam.Models.DTO_s.Role;

namespace FinalExam.Models.DTO_s.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<RoleDTO> Roles { get; set; }
        public Status Status { get; set; }
    }
}
