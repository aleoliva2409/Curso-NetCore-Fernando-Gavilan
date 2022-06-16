using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAuthors;
using WebApiAuthors.Filters;
using WebApiAuthors.Middlewares;
using WebApiAuthors.Services;
using WebApiAuthors.Utils;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
    // se agrega para el versionamiento de la API
    options.Conventions.Add(new SwaggeGroupByVersion());
}).AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// config swwager
builder.Services.AddSwaggerGen(c =>
{
    // Add the version and the title of your API.
    // se pueden agregar varios datos 
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebApiAuthors",
        Version = "v1",
        Description = "WebApiAuthors API(Curso .NET)",
        Contact = new OpenApiContact
        {
            Name = "Daniel Oliva",
            Email = "dannyoliva47@gmail.com",
            Url = new Uri("https://aleoliva2409-dev.com.ar/")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiAuthors", Version = "v2" });
    c.OperationFilter<AddHATEOASParameter>();
    c.OperationFilter<AddVersionParameter>();
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

    var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var filePath = Path.Combine(AppContext.BaseDirectory, fileXML);
    c.IncludeXmlComments(filePath);
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
        builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders(new string[] { "X-Total-Count" });
    });
});

builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();

// agregamos mas servicios
builder.Services.AddTransient<GenerateLinks>();
builder.Services.AddTransient<HATEOASAuthorFilterAttribute>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseMiddleware<LoggerMiddleware>();
app.UseLoggerMiddlewareResponse(); // se crea una clase para no exponer la original(LoggerMiddleware)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAuthors V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAuthors V2");
    });
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
