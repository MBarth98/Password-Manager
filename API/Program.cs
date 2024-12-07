using System.Reflection;
using System.Text;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        builder.Services.AddControllers();

        // Ensure db.db file exists
        var runtimeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.Combine(runtimeFolder, "db.db");
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<UserService, UserService>();
        builder.Services.AddScoped<PasswordService, PasswordService>();
        builder.Services.AddScoped<IPasswordRepository, PasswordRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        
        
        builder.Services.AddSingleton(new TokenService(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? builder.Configuration["JWT_SECRET_KEY"]));

        // Add JWT Authentication
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer =  builder.Configuration["JWT_ISSUER"],
                ValidAudience = builder.Configuration["JWT_AUDIENCE"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                    ??  builder.Configuration["JWT_SECRET_KEY"]))
            };
        });

        // Configure app
        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

