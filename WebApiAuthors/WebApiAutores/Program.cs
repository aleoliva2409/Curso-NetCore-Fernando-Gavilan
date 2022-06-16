using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAuthors;
using WebApiAuthors.Filters;
using WebApiAuthors.Middlewares;
using WebApiAuthors.Services;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
// Add services to the container.

builder.Services.AddControllers(options =>
    options.Filters.Add(typeof(ExceptionFilter))).AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// config swwager
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiAuthors", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
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
// agregamos servicio para autenticacion
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
        .GetBytes(builder.Configuration["secretKey"])),
        ClockSkew = TimeSpan.Zero
    });
builder.Services.AddAutoMapper(typeof(Program)); // Agregamos el mapper
// Configuramos Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
// agregando la authorization basada en claims
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IsAdmin", p => p.RequireClaim("isAdmin"));
});
// agregando la config dde CORS
builder.Services.AddCors(opt =>
{
    // recomendacion autocompletada
    //opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    opt.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseMiddleware<LoggerMiddleware>();
app.UseLoggerMiddlewareResponse(); // se crea una clase para no exponer la original(LoggerMiddleware)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Esto es para que funcione tanto en PROD como en DEV
/*app.UseSwagger();
app.UseSwaggerUI();*/

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(); // agregamos el middleware de CORS

app.UseAuthorization(); // agregamos el middleware para autenticacion

app.MapControllers();

app.Run();
