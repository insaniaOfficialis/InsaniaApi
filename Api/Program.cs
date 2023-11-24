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
using Services.Politics.Countries;
using Services.Identification.Authorization;
using Services.Identification.Registration;
using Services.Identification.Roles;
using Services.Identification.Token;
using Services.Initialization;
using System.Text;
using Files = Services.General.Files.Files;
using Services.Sociology.Races;
using Services.Sociology.Nations;
using Services.Sociology.PersonalNames;
using Api.Middleware;
using Services.General.Files;
using Services.General.Logs;
using Services.General.Logs.GetLogs;
using Services.Informations.InformationArticles.AddInformationArticle;
using Services.Informations.InformationArticlesDetails.AddInformationArticleDetail;
using Services.Informations.InformationArticles.GetInformationArticles;
using Services.General.Files.GetFile;
using Services.General.Files.GetFilesInformationArticleDetails;
using Services.Informations.InformationArticlesDetails.GetInformationArticleDetails;
using Services.Informations.News.GetNewsList;
using Services.General.Files.GetFilesNewsDetails;
using Services.Informations.NewsDetails.GetNewsDetails;
using Services.Informations.News.AddNews;
using Services.Informations.NewsDetails.AddNewsDetail;
using Services.General.Files.GetFilesUser;
using Microsoft.OpenApi.Models;
using Services.Informations.News.GetNewsFullList;
using Services.Informations.News.GetNewsTable;
using Services.Informations.News.EditNews;
using Services.Informations.NewsDetails.EditNewsDetail;
using Services.Informations.News.RemovalNews;
using Services.Informations.NewsDetails.RemovalNewsDetail;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var config = builder.Configuration;

//Вводим переменные для токена
var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["TokenOptions:Key"]!));
var issuer = config["TokenOptions:Issuer"];
var audience = config["TokenOptions:Audience"];

//Добавляем параметры для контекста базы данных
services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(config.GetConnectionString("DefaultPostgresConnectionString"));
    options.EnableSensitiveDataLogging();
});

//Добавляем параметры маппера моделей
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

//Добавляем параметры идентификации
builder.Services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationContext>();
builder.Services.AddControllersWithViews();

//Добавляем параметры авторизации
builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("Bearer")
        .RequireAuthenticatedUser().Build());
});

//Добавляем параметры сериализации и десериализации json
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

//Добавляем параметры политики паролей
services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});

//Добавляем параметры аутентификации
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

//Добавляем параметры логирования
Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.File(path: config["LoggingOptions:FilePath"]!, rollingInterval: RollingInterval.Day)
               .WriteTo.Debug()
               .CreateLogger();
services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));

//Добавляем параметры документации
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Insania API", Version = "v1" });

    var filePath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
    options.IncludeXmlComments(filePath);
});

//Внедряем зависимости для сервисов
builder.Services.AddScoped<IInitialization, Initialization>(); //инициализация
builder.Services.AddScoped<IRegistration, Registration>(); //регистрация
builder.Services.AddScoped<IRoles, Roles>(); //логика работы с ролями
builder.Services.AddScoped<IAuthorization, Authorization>(); //авторизация
builder.Services.AddScoped<IFiles, Files>(); //логика работы с файлами
builder.Services.AddScoped<IToken, Token>(); //логика формирования токена
builder.Services.AddScoped<ICountries, Countries>(); //логика работы со странами
builder.Services.AddScoped<IRaces, Races>(); //логика работы с расами
builder.Services.AddScoped<INations, Nations>(); //логика работы с нациями
builder.Services.AddScoped<IPersonalNames, PersonalNames>(); //логика работы с персональными именами
builder.Services.AddScoped<IGetLogs, GetLogs>(); //получение логов
builder.Services.AddScoped<IAddInformationArticle, AddInformationArticle>(); //добавление информационной статьи
builder.Services.AddScoped<IAddInformationArticleDetail, AddInformationArticleDetail>(); //добавление детальной части информационной статьи
builder.Services.AddScoped<IGetListInformationArticles, GetListInformationArticles>(); //получения списка информационных статей
builder.Services.AddScoped<IGetFile, GetFile>(); //получение файла
builder.Services.AddScoped<IGetFilesInformationArticleDetails, GetFilesInformationArticleDetails>(); //получение файлов детальной части информационной статьи
builder.Services.AddScoped<IGetInformationArticleDetails, GetInformationArticleDetails>(); //получение детальных частей информационной статьи
builder.Services.AddScoped<IGetNewsList, GetNewsList>(); //получение списка новостей
builder.Services.AddScoped<IGetFilesNewsDetails, GetFilesNewsDetails>(); //получение файлов детальной части новости
builder.Services.AddScoped<IGetNewsDetails, GetNewsDetails>(); //получение детальных частей новости
builder.Services.AddScoped<IAddNews, AddNews>(); //добавление новости
builder.Services.AddScoped<IAddNewsDetail, AddNewsDetail>(); //добавление детальной части новости
builder.Services.AddScoped<IGetFilesUser, GetFilesUser>(); //получение файлов пользователя
builder.Services.AddScoped<IGetNewsFullList, GetNewsFullList>(); //получение полного списка новостей
builder.Services.AddScoped<IGetNewsTable, GetNewsTable>(); //получение новостей для таблицы
builder.Services.AddScoped<IEditNews, EditNews>(); //редактирование новостей
builder.Services.AddScoped<IEditNewsDetail, EditNewsDetail>(); //редактирование детальной части новости
builder.Services.AddScoped<IRemovalNews, RemovalNews>(); //удаление/восстановление новости
builder.Services.AddScoped<IRemovalNewsDetail, RemovalNewsDetail>(); //удаление/восстановление детальной части новости

var app = builder.Build();

//Добавляем параетры конвеера запросов
app.UseMiddleware<LoggingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RegistrationController}/{action=Check}");
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Insania API V1");
});

app.MapGet("/", () => "Hello World!");

//Проводим первоначальную инициализацию
using var scope = app.Services.CreateScope();
var initialize = scope.ServiceProvider.GetService<IInitialization>();
await initialize!.InitializeDatabase();

app.Run();