using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewsService.API.Configs;
using NewsService.API.Extensions;
using NewsService.API.Middlewares;
using NewsService.BusinessLogic.Mapping;
using NewsService.BusinessLogic.Services;
using NewsService.BusinessLogic.Services.Interfaces;
using NewsService.DataAccess.Database;
using NewsService.DataAccess.Repositories;
using NewsService.DataAccess.Repositories.Interfaces;
using NewsService.Shared.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NewsService.API;

public class Startup
{
    private static string MyAllowSpecificOrigins { get; set; }
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<ITagsService, TagsService>();
        services.AddTransient<INewsService, NewsService.BusinessLogic.Services.NewsService>();
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<IDisciplineGrpcService, DisciplineGrpcService>();
        services.AddSingleton<IFileStorageConfig, FileStorageConfig>();
    }
    
    public static void ConfigureRepository(IServiceCollection services)
    {
        services.AddTransient<INewsRepository, NewsRepository>();
        services.AddTransient<ITagRepository, TagRepository>();
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
        services.AddDbContext<NewsContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("NewsService.API")));
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
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized (Token expired or invalid)");
                    }
                };
            });
    }

    public static void OptionsConfigure(IServiceCollection services, ConfigurationManager config)
    {
        services.Configure<GrpcDisciplineSettings>(config.GetSection("GrpcDisciplineSettings"));
    }

    public static void ConfigureRedis(IServiceCollection services, ConfigurationManager config)
    {
        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = config.GetConnectionString("Cache");
            option.InstanceName = "news";
        });
        services.AddSingleton<ICacheService, CacheService>();
    }
    
    public static void ConfigureCors(IServiceCollection services)
    {
        MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://frontend:3000")
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
            var dbContext = serviceProvider.GetService<NewsContext>();
            dbContext.Database.Migrate();
        }
    }
}
