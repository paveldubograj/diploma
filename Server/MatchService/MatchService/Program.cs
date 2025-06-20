using FluentValidation;
using FluentValidation.AspNetCore;
using MatchService.API;
using MatchService.API.Services;
using MatchService.BusinessLogic.Validators;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
Startup.OptionsConfigure(builder.Services, config);
Startup.ConfigureDataBase(builder.Services, config);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<MatchValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MatchUpdateDtoValidator>();
Startup.ConfigureRedis(builder.Services, config);
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
app.MapGrpcService<TournamentGrpcService>();
app.UseHttpsRedirection();

Startup.ConfigureCors(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
