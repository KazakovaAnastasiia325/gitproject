using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var users = new Dictionary<string, string>();  // Хранение пользователей (логин, пароль)
var sessions = new Dictionary<string, string>();  // Хранение сессий (sessionId, username)

var app = builder.Build();

// Использование Swagger в разработке
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();  // Статические файлы для HTML

// Роут для главной страницы
app.MapGet("/", async (HttpContext context) =>
{
    var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
    var html = await File.ReadAllTextAsync(htmlPath);
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(html);
});

// Роут для регистрации
app.MapPost("/register", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    // Проверка, существует ли пользователь с таким именем
    if (users.ContainsKey(username))
    {
        return Results.Json(new { success = false, message = "User already exists" });
    }

    // Добавление пользователя в список
    users[username] = password;
    return Results.Json(new { success = true, message = "Registration successful" });
});

// Роут для входа
app.MapPost("/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"];
    var password = form["password"];

    // Проверка правильности данных
    if (users.TryGetValue(username, out var storedPassword) && storedPassword == password)
    {
        // Генерация уникального sessionId
        var sessionId = Guid.NewGuid().ToString();
        sessions[sessionId] = username;
        return Results.Json(new { success = true, sessionId });
    }

    return Results.Json(new { success = false, message = "Invalid credentials" });
});

// Роут для приветствия
app.MapGet("/hello", (HttpContext context) =>
{
    if (!context.Request.Headers.TryGetValue("Session-Id", out var sessionId) || !sessions.ContainsKey(sessionId!))
    {
        return Results.Json(new { success = false, message = "Unauthorized" });
    }

    var messages = new[] {
        "Hello, World!",
        "Привет, мир!",
        "Сәлем, Әлем!",
        "Hola, Mundo!",
        "Bonjour, le monde!",
        "Hallo, Welt!",
        "こんにちは、世界！",
        "Ciao, mondo!",
        "안녕하세요, 세계!"
    };

    var random = new Random();
    return Results.Json(new { message = messages[random.Next(messages.Length)] });
});

// Роут для времени
app.MapGet("/time", (HttpContext context) =>
{
    if (!context.Request.Headers.TryGetValue("Session-Id", out var sessionId) || !sessions.ContainsKey(sessionId!))
    {
        return Results.Json(new { success = false, message = "Unauthorized" });
    }

    // Возвращаем текущее время
    return Results.Json(new { time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
});

// Запуск приложения
app.Run();