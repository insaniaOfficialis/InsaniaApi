using Data;
using Domain;
using Domain.Entities.Identification;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Services.Identification.Registration;
using Services.Identification.Roles;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var config = builder.Configuration;

services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(config.GetConnectionString("DefaultPostgresConnectionString"));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationContext>();
builder.Services.AddControllersWithViews();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});
builder.Services.AddScoped<IRegistration, Registration>();
builder.Services.AddScoped<IRoles, Roles>();

var app = builder.Build();

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RegistrationController}/{action=Check}");

app.MapGet("/", () => "Hello World!");

app.Run();