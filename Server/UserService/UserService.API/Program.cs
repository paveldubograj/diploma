using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.API;
using UserService.API.Services;
using UserService.BusinessLogic.Validators;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
Startup.OptionsConfigure(builder.Services, config);
Startup.ConfigureIdentity(builder.Services);
Startup.ConfigureDataBase(builder.Services, config);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>(); 
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAuthorization();
Startup.ConfigureCors(builder.Services);
Startup.ConfigureSwagger(builder.Services);
Startup.ConfigureAuth(builder.Services, config);

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

app.MapGrpcService<TournamentGrpcService>();
app.UseHttpsRedirection();
app.UseStaticFiles();

Startup.ConfigureCors(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

