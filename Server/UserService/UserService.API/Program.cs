using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.API;
using UserService.BusinessLogic.Validators;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
Startup.OptionsConfigure(builder.Services, config);
Startup.ConfigureIdentity(builder.Services);
Startup.ConfigureDataBase(builder.Services, config);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>(); 
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAuthorization();
//Startup.ConfigureCors(builder.Services);
Startup.ConfigureSwagger(builder.Services);

builder.Services.AddAuthentication(x =>
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["JwtSettings:Issuer"],
        ValidAudience = config["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456789012")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

Startup.ConfigureRepository(builder.Services);
Startup.ConfigureServices(builder.Services);

var app = builder.Build();
//Startup.UseMigrations(app);
Startup.InitializeRoles(builder.Services).Wait();
//Startup.ConfigureCors(app);
Startup.ConfigureMiddlewares(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.Use(async (context, next) =>
// {
//     var authHeader = context.Request.Headers["Authorization"];
//     Console.WriteLine($"Authorization Header: {authHeader}");
//     await next();
// });

// app.Use(async (context, next) =>
// {
//     var handler = new JwtSecurityTokenHandler();
//     var token = context.Request.Headers["Authorization"].ToString();
//     Console.WriteLine($"Raw Token: {token}");
//     //token = token.Substring("Bearer ".Length).Trim(); // Убираем "Bearer "
//     var jwtToken = handler.ReadJwtToken(token);
//     Console.WriteLine(jwtToken);
//     await next();
// });


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


















// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;

// var builder = WebApplication.CreateBuilder(args);

// var secretKey = "my_super_secret_key_123451234567"; // Секретный ключ (замени на свой)

// // Настройка аутентификации JWT
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//             ValidateIssuer = false,
//             ValidateAudience = false,
//             ValidateLifetime = true
//         };
//     });

// builder.Services.AddAuthorization();

// var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();

// // 📌 Эндпоинт для получения токена
// app.MapPost("/auth/login", ([FromBody] LoginRequest request) =>
// {
//     if (request.Username != "admin" || request.Password != "password")
//     {
//         return Results.Unauthorized();
//     }

//     var tokenHandler = new JwtSecurityTokenHandler();
//     var keyBytes = Encoding.UTF8.GetBytes(secretKey);

//     var tokenDescriptor = new SecurityTokenDescriptor
//     {
//         Subject = new ClaimsIdentity(new[]
//         {
//             new Claim(ClaimTypes.Name, request.Username),
//             new Claim(ClaimTypes.Role, "Admin") // Можно добавить роли
//         }),
//         Expires = DateTime.UtcNow.AddHours(1), // Время жизни токена
//         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
//     };

//     var token = tokenHandler.CreateToken(tokenDescriptor);
//     //var token = GenerateJwtToken(request.Username, secretKey);
//     var token1 = tokenHandler.WriteToken(token);
//     return Results.Ok(new { token1 });
// });

// // 📌 Защищенный эндпоинт (требует авторизации)
// app.MapGet("/secure/data", [Authorize] () =>
// {
//     return Results.Ok(new { message = "Секретные данные!" });
// });

// app.Run();

// // 📌 Модель запроса для входа
// public record LoginRequest(string Username, string Password);
