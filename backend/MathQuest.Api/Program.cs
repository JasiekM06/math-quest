using Microsoft.EntityFrameworkCore;
using MathQuest.Api.Data;

var builder = WebApplication.CreateBuilder(args); // Obiekt, który konfiguruje aplikację zanim wystartuje

builder.Services.AddDbContext<MathQuestContext>(options => options.UseSqlite("Data Source=mathquest.db")); // Rerestracja kontekstu bazy danych w systemie dependency injection

// Włączenie Swaggera
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "MathQuest API",
        Version = "v1",
        Description = "API do aplikacji z zadaniami matematycznymi dla dzieci"
    });
});

builder.Services.AddControllers(); // rejestruje system Controllerów w dependency injection

var app = builder.Build(); // Buduje gotową aplikację

if (app.Environment.IsDevelopment()) // Włącza Swaggera tylko w trybie deweloperskim
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Jeśli ktoś wejdzie przez http://, automatycznie zostaje przekierowany na https://
app.MapControllers(); // Mówi aplikacji, eby zmapowała wszystkie endpointy znalezione w Controllerach na odpowiednie adresy URL

app.MapGet("/", () => "MathQuest API działa!");

app.Run(); // Uruchamia serwer i czeka na requesty