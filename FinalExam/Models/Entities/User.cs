using FinalExam.Enums;
using FinalExam.Models.Entities;
using FinalExam.Models;
using System.ComponentModel.DataAnnotations;

public class User : BaseClass
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    public byte[] PasswordHash { get; set; }

    [Required]
    public byte[] PasswordSalt { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationDate { get; set; }

    [Required]
    public List<Role> Roles { get; set; } = new List<Role>();

    public Status Status { get; set; } = Status.Active;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public List<Booking> Bookings { get; set; } = new();
}
