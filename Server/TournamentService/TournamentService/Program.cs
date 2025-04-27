using FluentValidation;
using FluentValidation.AspNetCore;
using TournamentService.API;
using TournamentService.BusinessLogic.Validators;
using TournamentService.DataAccess.Entities;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
Startup.OptionsConfigure(builder.Services, config);
Startup.ConfigureDataBase(builder.Services, config);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<ParticipantValidator>(); 
builder.Services.AddValidatorsFromAssemblyContaining<TournamentValidator>(); 
builder.Services.AddFluentValidationAutoValidation();
Startup.ConfigureCors(builder.Services);
Startup.ConfigureSwagger(builder.Services);
Startup.ConfigureAuth(builder.Services, config);
Startup.ConfigureRepository(builder.Services);
Startup.ConfigureServices(builder.Services);

var app = builder.Build();
//Startup.UseMigrations(app);

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
