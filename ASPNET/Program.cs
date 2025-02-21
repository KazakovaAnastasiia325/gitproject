using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/hello", () =>
{
    var mess = new[]
    {
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
    return Results.Json(new { mess = mess[random.Next(mess.Length)] });
});

app.MapGet("/time", () => Results.Json(new { time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }));

app.Run();