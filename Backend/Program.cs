using Backend.DataContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
string cadenaConexion = configuration.GetConnectionString("mysqlRemoto");

// configuración de inyección de dependencias del DBContext
builder.Services.AddDbContext<ParkARContext>(
    options => options.UseMySql(cadenaConexion,
                                ServerVersion.AutoDetect(cadenaConexion)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
