using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using TournamentService.API.Extensions;
using TournamentService.API.Middlewares;
using TournamentService.BusinessLogic.Mapping;
using TournamentService.BusinessLogic.Services;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.BusinessLogic.Services.Tournaments;
using TournamentService.BusinessLogic.Services.Tournaments.Interfaces;
using TournamentService.DataAccess.Database;
using TournamentService.DataAccess.Repositories;
using TournamentService.DataAccess.Repositories.Interfaces;
using TournamentService.Shared.Options;

namespace TournamentService.API;

public class Startup
{
private static string MyAllowSpecificOrigins { get; set; }
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IParticipantService, ParticipantService>();
        services.AddTransient<ITournamentService, TournamentService.BusinessLogic.Services.TournamentService>();
        services.AddTransient<IParticipantService, ParticipantService>();
        services.AddTransient<ISingleEliminationBracket, SingleEliminationBracket>();
        services.AddTransient<IRoundRobinBracket, RoundRobinBracket>();
        services.AddTransient<ISwissBracket, SwissBracket>();
        services.AddTransient<IDoubleEliminationBracket, DoubleEliminationBracket>();
    }
    
    public static void ConfigureRepository(IServiceCollection services)
    {
        services.AddTransient<IParticipantRepository, ParticipantRepository>();
        services.AddTransient<ITournamentRepository, TournamentRepository>();
    }

    
    public static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigurationExtension>();
    }
    
    public static void ConfigureDataBase(IServiceCollection services, ConfigurationManager config)
    {
        var connectionString = config.GetConnectionString("DataBase");
        services.AddDbContext<TournamentContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("TournamentService.API")), 
            ServiceLifetime.Singleton, 
            ServiceLifetime.Singleton);
    }
    
    public static void ConfigureMiddlewares(WebApplication app)
    {
        app.UseMiddleware<ExceptionAndLoggingMiddleware>();
    }
    
    public static void ConfigureAuth(IServiceCollection services, ConfigurationManager config)
    {
        services.AddAuthentication(x =>
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

    }

    public static void OptionsConfigure(IServiceCollection services, ConfigurationManager config)
    {
        services.Configure<JwtOption>(config.GetSection("JwtSettings"));
    }
    
    public static void ConfigureCors(IServiceCollection services)
    {
        MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }

    public static void ConfigureCors(WebApplication app)
    {
        app.UseCors(MyAllowSpecificOrigins);
    }

    public static void UseMigrations(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var dbContext = serviceProvider.GetService<TournamentContext>();
            dbContext.Database.Migrate();
        }
    }
}
