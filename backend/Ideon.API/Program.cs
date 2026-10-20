using Ideon.API.Data;
using Microsoft.EntityFrameworkCore;
using Ideon.API.Services;
using Ideon.API.Interfaces;
using Ideon.API.Services.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// controller services are added to the dependency injection container, allowing them to be discovered and used by the application.
builder.Services.AddControllers();
//these services are added to the dependency injection container, allowing them to be injected into controllers and other services that require them.
builder.Services.AddScoped<IAuthService, AuthService>();
// Add JWT authentication configuration
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
//idea service added to the dependency injection container, allowing it to be injected into controllers and other services that require it.
builder.Services.AddScoped<IIdeaService, IdeaService>();
builder.Services.AddScoped<ICategoryService, CategoryService>(); //category service added to the dependency injection container, allowing it to be injected into controllers and other services that require it.
builder.Services.AddScoped<ICommentService, CommentService>();//comment service added to the dependency injection container, allowing it to be injected into controllers and other services that require it.
builder.Services.AddScoped<IRatingService, RatingService>();//rating service added to the dependency injection container, allowing it to be injected into controllers and other services that require it.
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
// Enable authentication and authorization middleware in the request pipeline, allowing the application to handle authentication and authorization for incoming requests.
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
//app.MapGet("/ideas", async (IIdeaService ideaService) =>
app.MapControllers();

app.Run();