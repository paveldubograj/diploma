using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MatchService.API.Extensions;
using MatchService.API.Middlewares;
using MatchService.BusinessLogic.Mapping;
using MatchService.BusinessLogic.Services.Interfaces;
using MatchService.DataAccess.Database;
using MatchService.DataAccess.Repositories;
using MatchService.DataAccess.Repositories.Interfaces;
using MatchService.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MatchService.API;

public class Startup
{
    private static string MyAllowSpecificOrigins { get; set; }
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<IMatchService, BusinessLogic.Services.MatchService>();
    }
    
    public static void ConfigureRepository(IServiceCollection services)
    {
        services.AddTransient<IMatchRepository, MatchRepository>();
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
        services.AddDbContext<MatchContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("MatchService.API")));
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"])),
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
            var dbContext = serviceProvider.GetService<MatchContext>();
            dbContext.Database.Migrate();
        }
    }
}
