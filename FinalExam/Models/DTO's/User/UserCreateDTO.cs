namespace FinalExam.Models.DTO_s.User
{
    public class UserCreateDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
