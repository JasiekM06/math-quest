namespace MathQuest.Api.Models;

// Model uytkownika w aplikacji
public class User 
{
    public int UserId {get; set; } // klucz główny (ID uytkownika w bazie danych) 
    public string Username { get; set; } = string.Empty; // Nazwa uytkownika (login)
    public string PasswordHash { get; set; } = string.Empty; // Hash hasła (NIE trzymamy zwykłego hasła)
    public DateTime CreatedAt { get; set; } // Data utworzenia konta
    public ICollection<UserTaskStatistic> Statistics { get; set; } = new List<UserTaskStatistic>(); // Relacja 1 user -> wiele statystyk zadań
}