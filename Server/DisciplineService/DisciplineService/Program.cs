using System.Text;
using DisciplineService.API;
using DisciplineService.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
Startup.OptionsConfigure(builder.Services, config);
Startup.ConfigureDataBase(builder.Services, config);

builder.Services.AddControllers();
builder.Services.AddGrpc();
Startup.ConfigureCors(builder.Services);
Startup.ConfigureSwagger(builder.Services);
Startup.ConfigureRepository(builder.Services);
Startup.ConfigureServices(builder.Services);
Startup.ConfigureAuth(builder.Services, config);

var app = builder.Build();
//Startup.UseMigrations(app);


Startup.ConfigureMiddlewares(app);
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGrpcService<DisciplineService.API.Services.DisciplineService>();
Startup.ConfigureCors(app);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();