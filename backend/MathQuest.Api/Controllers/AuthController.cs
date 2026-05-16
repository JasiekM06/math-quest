using MathQuest.Api.Data;
using MathQuest.Api.DTOs;
using MathQuest.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MathQuest.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly MathQuestContext _db;
    private readonly IConfiguration _config;

    public AuthController(MathQuestContext db, IConfiguration config)
    {
        _db = db;
        _config = config; // Przechowujemy konfigurację, zeby móc odczytać klucz JWT
    }

    // Endpoint rejestracji
    [HttpPost("register")]
    [EndpointSummary("Rejestracja nowego uzytkownika")]
    [EndpointDescription("Tworzy nowe konto uzytkownika. Hasło jest hashowane przez BCrypt. Zwraca 400 jeśli uzytkownik o podanej nazwie juz istnieje.")]
    [ProducesResponseType(StatusCodes.Status200OK)] // ProducesResponseType mówi Swaggerowi jakie kody HTTP moze zwrócić ten endpoint
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto Dto)
    {
        var exist = await _db.Users
            .AnyAsync(u => u.Username == Dto.Username);

        if (exist)
            return BadRequest("Uzytkownik o tej nazwie juz istnieje.");

        var user = new User
        {
            Username = Dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok("Rejestracja zakończona sukcesem!");     
    }

    // Endpoint logowania
    [HttpPost("login")]
    [EndpointSummary("Logowanie uzytkownika")]
    [EndpointDescription("Sprawdza dane logowania i zwraca dane uzytkownika. Zwraca 401 jeśli nazwa uzytkownika lub hasło są nieprawidłowe.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username); // Szuka uzytkowników po nazwie, zwraca null jeśli nie znalazł

        if (user == null)
            return Unauthorized("Nieprawidłowa nazwa uytkownika lub hasło.");

        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash); // Porównuje wpisane hasło z hashem w bazie. Nigdy nie "odszyfrowujemy" hasła, tylko sprawdzamy czy hash się zgadza

        if (!passwordValid)
            return Unauthorized("Nieprawidłowa nazwa uzytkownika lub hasło.");

        var token = GenerateJwtToken(user);
        return Ok(new { userId = user.UserId, username = user.Username, token = token }); // Zwracamy anonimowy obiekt z danymi uytkownika bez PasswordHash!
    }

    private string GenerateJwtToken(User user)
    {
        // Pobieramy klucz z konfiguracji i tworzymy obiekt klucza
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        // Określamy algorytm podpisywania tokena
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claim to informacje które pakujemy do tokena
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Sub (Subject) - kto jest właścicielem tokena, zapisujemy ID uzytkownika
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username), // UniqueName - nazwa uzytkownika, frontend moze ją odczytać z tokena
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Jti (JWT ID) - unikalny identyfikator tego konkretnego tokena, zapobiega duplikatom
        };

        // Tworzymy token z wszystkimi parametrami
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24), // Token wygasa po 24 godzinach
            signingCredentials: creds
        );

        // Zamieniamy token na string, który wysyłamy do frontendu
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}