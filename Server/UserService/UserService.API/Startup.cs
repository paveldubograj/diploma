using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using UserService.API.Extensions;
using UserService.API.Middlewares;
using UserService.BusinessLogic.Mapping;
using UserService.BusinessLogic.Services;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.DataAccess.DataBase;
using UserService.DataAccess.Entities;
using UserService.DataAccess.Repositories;
using UserService.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Shared.Contants;
using UserService.Shared.Options;

namespace UserService.API;

public class Startup
{
    private static string MyAllowSpecificOrigins { get; set; }
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserManageService, UserManageService>();
        services.AddTransient<IRolesService, RolesService>();
    }
    
    public static void ConfigureRepository(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
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
        services.AddDbContext<UsersContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("UserService.API")));
    }

    public static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<UsersContext>()
            .AddUserManager<UserManager<User>>()
            .AddDefaultTokenProviders();
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

    }
    
    public static async Task InitializeRoles(IServiceCollection services)
    {
        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            await EnsureRoleCreated(roleManager, RoleName.User);
            await EnsureRoleCreated(roleManager, RoleName.Admin);
            await EnsureRoleCreated(roleManager, RoleName.Organizer);
            await EnsureRoleCreated(roleManager, RoleName.NewsTeller);
        }
    }
    
    private static async Task EnsureRoleCreated(RoleManager<IdentityRole> roleManager, string roleName)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    public static void OptionsConfigure(IServiceCollection services, ConfigurationManager config)
    {
        services.Configure<JwtOptions>(config.GetSection("JwtSettings"));
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
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
    }

    public static void ConfigureCors(WebApplication app)
    {
        app.UseCors(MyAllowSpecificOrigins);
    }

    // public static void UseMigrations(WebApplication app)
    // {
    //     using (var scope = app.Services.CreateScope())
    //     {
    //         var serviceProvider = scope.ServiceProvider;
    //         var dbContext = serviceProvider.GetService<UsersContext>();
    //         dbContext.Database.Migrate();
    //     }
    // }
}
