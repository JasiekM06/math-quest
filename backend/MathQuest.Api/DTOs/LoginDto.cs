using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MathQuest.Api.DTOs;

public class LoginDto
{
    [Required]
    [Description("Nazwa uzytkownika podana przy rejestracji")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Description("Hasło podane przy rejestracji")]
    public string Password { get; set; } = string.Empty;
}