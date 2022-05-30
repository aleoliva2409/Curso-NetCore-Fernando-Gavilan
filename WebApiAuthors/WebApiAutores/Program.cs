using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAuthors;
using WebApiAuthors.Filters;
using WebApiAuthors.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
// Add services to the container.

builder.Services.AddControllers(options => 
    options.Filters.Add(typeof (ExceptionFilter))).AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); // agregamos servicio para autenticacion

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseMiddleware<LoggerMiddleware>();
app.UseLoggerMiddlewareResponse(); // se crea una clase para no exponer la original(LoggerMiddleware)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization(); // agregamos el middleware para autenticacion

app.MapControllers();

app.Run();
