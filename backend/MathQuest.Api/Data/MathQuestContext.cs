using Microsoft.EntityFrameworkCore;
using MathQuest.Api.Models;

namespace MathQuest.Api.Data;

// Główny kontekst bazy danych (EF Core)
public class MathQuestContext : DbContext
{
    public MathQuestContext(DbContextOptions<MathQuestContext> options) : base(options) // Konstruktor przekazujący konfigurację
    {
    }

    public DbSet<User> Users { get; set; } // Tabela Users w bazie danych
    public DbSet<MathTask> Tasks {get; set; } // Tabela zadań matematycznych
    public DbSet<UserTaskStatistic> Statistics { get; set; } // Tabela statystyk uytkownika

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Konfiguracja relacji i ograniczeń bazy danych
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserTaskStatistic>() // RELACJA: UserTaskStatistic -> User (wiele statystyk do jednego usera)
        .HasOne(s => s.User)
        .WithMany(u => u.Statistics)
        .HasForeignKey (s => s.UserId)
        .OnDelete(DeleteBehavior.Cascade); // usunięcie usera usuwa statystyki

        modelBuilder.Entity<UserTaskStatistic>() // RELACJA: UserTastStatistic -> MathTask
        .HasOne(s => s.Task)
        .WithMany(t => t.Statistic)
        .HasForeignKey(s => s.TaskId)
        .OnDelete(DeleteBehavior.Cascade); // usunięcie zadania usuwa statystyki

        modelBuilder.Entity<User>() // Username musi być unikalny
        .HasIndex(u => u.Username)
        .IsUnique();

        modelBuilder.Entity<UserTaskStatistic>() // Jeden user moze mieć tylko jeden wpis statystyki na jedno zadanie
        .HasIndex(s => new { s.UserId, s.TaskId })
        .IsUnique();
    }
}

