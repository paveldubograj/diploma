using FluentValidation;
using FluentValidation.AspNetCore;
using NewsService.API;
using NewsService.BusinessLogic.Validators;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
Startup.OptionsConfigure(builder.Services, config);
Startup.ConfigureDataBase(builder.Services, config);

builder.Services.AddValidatorsFromAssemblyContaining<NewsValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TagValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();
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
app.UseStaticFiles();

Startup.ConfigureCors(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();