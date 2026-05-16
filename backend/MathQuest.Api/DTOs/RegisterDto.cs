using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MathQuest.Api.DTOs;

public class RegisterDto // Te rzeczy uzytkownik wysyła przy rejestracji
{
    [Required] // Mówi swaggerowi, ze pole jest wymagane
    [MinLength(3)]
    [MaxLength(20)]
    [Description("Nazwa uzytkownika (login)")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [Description("Hasło uzytkownika (minimum 6 znaków)")]
    public string Password { get; set; } = string.Empty;
}