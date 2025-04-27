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
Startup.ConfigureCors(builder.Services);
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
Startup.ConfigureMiddlewares(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

Startup.ConfigureCors(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

