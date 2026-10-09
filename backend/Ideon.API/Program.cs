using Ideon.API.Data;
using Microsoft.EntityFrameworkCore;
using Ideon.API.Services;
using Ideon.API.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// controller services are added to the dependency injection container, allowing them to be discovered and used by the application.
builder.Services.AddControllers();
//idea service added to the dependency injection container, allowing it to be injected into controllers and other services that require it.
builder.Services.AddScoped<IIdeaService, IdeaService>();
builder.Services.AddScoped<ICategoryService, CategoryService>(); //category service added to the dependency injection container, allowing it to be injected into controllers and other services that require it.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy to allow requests from the frontend application running on http://localhost:5173. cors is a security feature implemented by web browsers that restricts web pages from making requests to a different domain than the one that served the web page. By adding this CORS policy, the backend API allows requests from the specified frontend origin, enabling communication between the frontend and backend during development.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Frontend");// Use the configured CORS policy for incoming requests, allowing requests from the specified frontend origin.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
//app.MapGet("/ideas", async (IIdeaService ideaService) =>
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
