using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Domains;
using TaskManager.Api.Services.Implementation;
using TaskManager.Api.Services.Interfaces;
using TaskManager.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Добавляем контроллеры
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Подключение к БД
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                          "Server=(localdb)\\mssqllocaldb;Database=TaskManagerDb;Trusted_Connection=True;";

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<ApplicationContext>();

// Настройка JWT аутентификации
// Добавляем сервис аутентификации в контейнер зависимостей.
builder.Services.AddAuthentication(options =>
    {
        // Устанавливаем схему аутентификации по умолчанию для проверки подлинности токенов JWT.
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

        // Устанавливаем схему аутентификации по умолчанию для вызова вызова (challenge) при неудачной аутентификации.
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
// Добавляем поддержку JWT-токенов в механизм аутентификации.
    .AddJwtBearer(options =>
    {
        // Настройка параметров валидации JWT-токена.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Включаем проверку издателя токена (issuer). Это гарантирует, что токен был выпущен доверенным источником.
            ValidateIssuer = true,

            // Включаем проверку получателя токена (audience). Это гарантирует, что токен предназначен для нашего приложения.
            ValidateAudience = true,

            // Включаем проверку времени жизни токена. Это гарантирует, что токен не истёк.
            ValidateLifetime = true,

            // Включаем проверку ключа подписи токена. Это гарантирует, что токен был подписан доверенным ключом.
            ValidateIssuerSigningKey = true,

            // Указываем допустимого издателя токена (issuer), который берётся из конфигурации приложения.
            ValidIssuer = builder.Configuration["JWT:Issuer"],

            // Указываем допустимую аудиторию токена (audience), которая также берётся из конфигурации приложения.
            ValidAudience = builder.Configuration["JWT:Audience"],

            // Указываем ключ, используемый для подписи токена. 
            // Ключ берётся из конфигурации приложения и преобразуется в массив байтов с использованием UTF-8 кодировки.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"))),
            RoleClaimType = ClaimTypes.Role // Явно указываем тип claim для роли
        };
    });

// Добавляем PasswordHasher в контейнер зависимостей
builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IDeskService, DeskService>();
builder.Services.AddScoped<ITaskService, TaskService>();
//builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Настраиваем авторизацию
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin", "Editor"));
    options.AddPolicy("UserOnly", policy => 
        policy.RequireRole("User"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationContext>();
        var passwordHasher = services.GetRequiredService<PasswordHasher<User>>();

        // Создаем базу данных, если она не существует
        context.Database.EnsureCreated();

        // Проверяем есть ли админ в системе
        var existingAdmin = context.Users.FirstOrDefault(u => u.Login == "admin");
        if (existingAdmin != null)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Updating existing admin user with status {UserStatus}", existingAdmin.UserStatus);
            
            // Обновляем статус существующего администратора
            existingAdmin.UserStatus = UserStatus.Admin;
            context.Users.Update(existingAdmin);
            context.SaveChanges();
            
            logger.LogInformation("Updated admin user with status {UserStatus}", existingAdmin.UserStatus);
        }
        else
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Creating new admin user...");

            var admin = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Login = "admin",
                Email = "admin@gmail.com",
                Password = "123", // Временный пароль
                Phone = "+79093070777",
                UserStatus = UserStatus.Admin,
                RegistrationDate = DateTime.Now
            };

            // Хешируем пароль
            admin.Password = passwordHasher.HashPassword(admin, "123");

            context.Users.Add(admin);
            context.SaveChanges();

            logger.LogInformation("Created new admin user with status {UserStatus}", admin.UserStatus);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации базы данных.");
    }
}

// Настраиваем маршрутизацию
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Запуск приложения
app.Run();