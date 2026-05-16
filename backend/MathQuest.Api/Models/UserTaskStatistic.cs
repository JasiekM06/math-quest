namespace MathQuest.Api.Models;

// Model łączący User + MathTask + dane o próbach
public class UserTaskStatistic
{
    public int UserTaskStatisticId { get; set; } // ID rekordu statystyki (klucz główny)
    public int UserId { get; set; } // ID uzytkownika (klucz obcy)
    public int TaskId { get; set; } // ID zadania (klucz obcy)
    public int AttemptCount {get; set; } = 1; // Liczba prób rozwiązania zadania
    public bool IsCompleted { get; set; } = false; // Czy zadanie zostało ukończone poprawnie
    public DateTime FirstAttemptTime { get; set; } // Czas pierwszej próby
    public DateTime LastAttemptTime { get; set; } // Czas ostatniej próby

    public User User { get; set; } = null!; // Nawigacja: do uzytkownika (EF Core wypełnia automatycznie)
    public MathTask Task { get; set; } = null!; // Nawigacja: do zadania (MathTask)
}