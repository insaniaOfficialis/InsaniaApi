using Data;
using Domain;
using Domain.Entities.Identification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Services.Files;
using Services.Geography.Countries;
using Services.Identification.Authorization;
using Services.Identification.Registration;
using Services.Identification.Roles;
using Services.Identification.Token;
using Services.Initialization;
using System.Text;
using Files = Services.Files.Files;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var config = builder.Configuration;

/*Вводим переменные для токена*/
var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["TokenOptions:Key"]!));
var issuer = config["TokenOptions:Issuer"];
var audience = config["TokenOptions:Audience"];

/*Добавляем параметры для контекста базы жанных*/
services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(config.GetConnectionString("DefaultPostgresConnectionString"));
    options.EnableSensitiveDataLogging();
});

/*Добавляем параметры маппера моделей*/
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

/*Добавляем параметры идентификации*/
builder.Services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationContext>();
builder.Services.AddControllersWithViews();

/*Добавляем параметры авторизации*/
builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("Bearer")
        .RequireAuthenticatedUser().Build());
});

/*Добавляем параметры сериализации и десериализации json*/
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

/*Добавляем параметры политики паролей*/
services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});

/*Добавляем параметры аутентификации*/
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = issuer,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = audience,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = key,
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

/*Добавляем параметры логирования*/
Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.File(path: config["LoggingOptions:FilePath"]!, rollingInterval: RollingInterval.Day)
               .WriteTo.Debug()
               .CreateLogger();
services.AddLogging(loggingBuilder =>
  loggingBuilder.AddSerilog(Log.Logger, dispose: true));

/*Внедряем зависимости для сервисов*/
builder.Services.AddScoped<IInitialization, Initialization>();
builder.Services.AddScoped<IRegistration, Registration>();
builder.Services.AddScoped<IRoles, Roles>();
builder.Services.AddScoped<IAuthorization, Authorization>();
builder.Services.AddScoped<IFiles, Files>();
builder.Services.AddScoped<IToken, Token>();
builder.Services.AddScoped<ICountries, Countries>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RegistrationController}/{action=Check}");

app.MapGet("/", () => "Hello World!");

/*Проводим первоначальную инициализацию*/
using var scope = app.Services.CreateScope();
var initialize = scope.ServiceProvider.GetService<IInitialization>();
var success = await initialize!.InitializeDatabase();

app.Run();