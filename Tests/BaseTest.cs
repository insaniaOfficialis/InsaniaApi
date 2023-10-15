using Data;
using Domain.Entities.Identification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Services.Identification.Token;
using System.Text;

namespace Tests;

/// <summary>
/// Базовый тест
/// </summary>
public class BaseTest
{
    public ServiceCollection _serviceCollections; //коллеция сервис
    public ApplicationContext _repository; //репозиторий сущности
    public IMapper _mapper; //интерфейс преобразования моделей
    public UserManager<User> _userManager; //менеджер пользователей
    public IToken _token; //сервис токенов

    /// <summary>
    /// Конструктор базового теста
    /// </summary>
    public BaseTest()
    {
        //Объявляем новую коллекцию сервисов
        _serviceCollections = new ServiceCollection();

        //Добавляем параметры для контекста базы данных
        _serviceCollections.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql("Server=localhost;Port=5432;Database=Insania;Username=postgres;Password=111;");
            options.EnableSensitiveDataLogging();
        });

        //Добавляем параметры маппера моделей
        _serviceCollections.AddAutoMapper(typeof(AppMappingProfile));

        //Добавляем параметры идентификации
        _serviceCollections.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationContext>();
        _serviceCollections.AddControllersWithViews();

        //Добавляем параметры авторизации
        _serviceCollections.AddAuthorization(auth =>
        {
            auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Bearer")
                .RequireAuthenticatedUser().Build());
        });

        //Добавляем параметры сериализации и десериализации json
        _serviceCollections.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.WriteIndented = true;
        });

        //Добавляем параметры политики паролей
        _serviceCollections.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        });

        //Добавляем параметры аутентификации
        _serviceCollections.AddAuthentication(options => {
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
                    ValidIssuer = "Api",
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // установка потребителя токена
                    ValidAudience = "Client",
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    // установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes("Altair von Alfheim Demiurge of Insanity and Lord of the Halls of Reason")),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                };
            });

        //Внедряем зависимости для сервисов
        _serviceCollections.AddScoped<IToken, Token>();  
        
        //Добавляем сервис конфигураций
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        IConfiguration configuration = builder.Build();
        _serviceCollections.AddScoped(_ => configuration);

        //Добавляем параметры логирования
        Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Verbose()
                       .WriteTo.File(path: "Logs\\Tests\\log.txt", rollingInterval: RollingInterval.Day)
                       .WriteTo.Debug()
                       .CreateLogger();
        _serviceCollections.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));

        //Добавляем параметры маппера моделей
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AppMappingProfile());
        });
        IMapper mapper = mappingConfig.CreateMapper();
        _mapper = mapper;

        //Строим сервисы
        var serviceProvider = _serviceCollections.BuildServiceProvider();

        //Получаем менеджер пользователей
        _userManager = serviceProvider.GetService<UserManager<User>>()!;
        _token = serviceProvider.GetService<IToken>()!;
        _repository = serviceProvider.GetService<ApplicationContext>()!;
    }
}