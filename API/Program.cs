using System.Text;
using API;
using Application;
using Application.DTOs;
using Domain;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("Data Source=app.db"));
        });

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        IdentityBuilder ibuilder = builder.Services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
            }
        );
        ibuilder = new IdentityBuilder(ibuilder.UserType, typeof(IdentityRole), builder.Services);
        ibuilder.AddEntityFrameworkStores<ApplicationDbContext>();
        //.AddDefaultTokenProviders();

        ibuilder.AddRoleValidator<RoleValidator<IdentityRole>>();
        ibuilder.AddRoleManager<RoleManager<IdentityRole>>();
        ibuilder.AddSignInManager<SignInManager<User>>();
        ibuilder.AddUserManager<UserManager<User>>();               
        
        // JWT Authentication Configuration
        var key = "very_secret_key_very_secret_key_very_secret_key"u8.ToArray();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token successfully validated.");
                        return Task.CompletedTask;
                    }
                };
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateLifetime = false,
                    ValidateAudience = false
                };
            });

        // Authorization setup
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IAuthorizationHandler, Authorization>();
        builder.Services.AddHttpContextAccessor(); 
        

        // Add services to the container.
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<IRepository<User>, UserRepository>();
        builder.Services.AddScoped<IRepository<Password>, PasswordRepository>();

        builder.Services.AddControllers();
        
        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ng",
                corsBuilder => corsBuilder
                    .WithOrigins("http://localhost:4200") // Ensure this matches the Angular app URL exactly
                    .AllowAnyMethod() // Allow all HTTP methods
                    .AllowAnyHeader() // Allow all headers
                    .AllowCredentials() // Allow sending credentials (cookies or authorization headers)
            );
        });
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: '12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
        
        var app = builder.Build();
        
        // Ensure db created
        using (var serviceScope = app.Services.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        }

        app.UseHttpsRedirection(); // Ensures requests are redirected to HTTPS
        app.UseCors("ng");

        app.UseAuthentication();
        app.UseAuthorization(); 

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.MapControllers();
        app.Run();
    }
}

