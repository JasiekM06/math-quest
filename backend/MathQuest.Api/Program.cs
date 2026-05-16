using Microsoft.EntityFrameworkCore;
using MathQuest.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args); // Obiekt, który konfiguruje aplikację zanim wystartuje

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true); // Ładuje lokalną konfigurację (np. klucz JWT) która nadpisuje appsettings.json

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

// Pobieramy ustawienia JWT z konfiguracji (appsettings.json / appsettings.Local.json)
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

// Konfigurujemy uwierzytelnianie JWT - aplikacja będzie weryfikować tokeny w requestach
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Sprawdza kto wydał token (nasz serwer)
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            // Sprawdzaj dla kogo jest token (nasz frontend)
            ValidateAudience = true,
            ValidAudience = jwtAudience,

            // Sprawdza czy token nie wygasł
            ValidateLifetime = true,

            // Sprawdza czy podpis tokena jest poprawny
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build(); // Buduje gotową aplikację

if (app.Environment.IsDevelopment()) // Włącza Swaggera tylko w trybie deweloperskim
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Jeśli ktoś wejdzie przez http://, automatycznie zostaje przekierowany na https://
app.UseAuthentication(); // Włącza uwierzytelnianie - sprawdza czy request ma wany token
app.UseAuthorization(); // Włącza autoryzację - sprawdza, czy uytkownik ma dostęp do danego endpointu
app.MapControllers(); // Mówi aplikacji, eby zmapowała wszystkie endpointy znalezione w Controllerach na odpowiednie adresy URL

app.MapGet("/", () => "MathQuest API działa!");

app.Run(); // Uruchamia serwer i czeka na requesty