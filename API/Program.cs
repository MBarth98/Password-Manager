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

        builder.Services.AddDbContext<ApplicationDbContext>(options => {
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

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
                ValidIssuer =  "iss",
                ValidAudience = "aud",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                    ??  builder.Configuration["JWT_SECRET_KEY"]))
            };
        });

        builder.Services.AddHttpContextAccessor(); // If not already added

        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularLocalhost",
                corsBuilder => corsBuilder
                    .WithOrigins("http://localhost:4200") // Ensure this matches the Angular app URL exactly
                    .AllowAnyMethod() // Allow all HTTP methods
                    .AllowAnyHeader() // Allow all headers
                    .AllowCredentials() // Allow sending credentials (cookies or authorization headers)
            );
        });

        // Configure app
        var app = builder.Build();

        using (var serviceScope = app.Services.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        app.UseCors("AllowAngularLocalhost"); // Apply CORS policy before authentication

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

