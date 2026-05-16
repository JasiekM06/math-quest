namespace MathQuest.Api.Models;

// Model zadania matematycznego
public class MathTask
{
    public int MathTaskId { get; set; } // ID zadania (klucz główny)
    public string Content { get; set; } = string.Empty; // Treść zadania (np. "2 + 2 = ?")
    public int CorrectAnswer { get; set; } // Poprawna odpowiedź
    public int Difficulty { get; set; } = 1; // Poziom trudności (1 = łatwe)
    public DateTime CreatedAt { get; set; } // Data utworzenia zadania

    public ICollection<UserTaskStatistic> Statistic { get; set; } = new List<UserTaskStatistic>(); // Relacja 1 zadanie -> wiele statystyk uytkowników
}