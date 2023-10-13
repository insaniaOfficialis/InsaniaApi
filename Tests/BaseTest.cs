using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Tests;

/// <summary>
/// Базовый тест
/// </summary>
public class BaseTest
{
    public ServiceCollection _serviceCollections; //коллеция сервис
    public ApplicationContext _repository; //репозиторий сущности
    public IMapper _mapper; //интерфейс преобразования моделей

    /// <summary>
    /// Конструктор базового теста
    /// </summary>
    public BaseTest()
    {
        //Объявляем новую коллекцию сервисов
        _serviceCollections = new ServiceCollection();

        //Добавляем параметры для контекста базы данных
        string connection = "Server=localhost;Port=5432;Database=Insania;Username=postgres;Password=111;";
        var options = new DbContextOptionsBuilder<ApplicationContext>()
             .UseNpgsql(connection)
             .Options;
        _repository = new ApplicationContext(options);

        //Добавляем параметры маппера моделей
        _serviceCollections.AddAutoMapper(typeof(AppMappingProfile));

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
    }
}
