using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Hubs;
using TeamFinder.Api.Services;
var builder = WebApplication.CreateBuilder(args);

// 1. AÑADIR SERVICIOS AL CONTENEDOR

// Registra los controladores de API y MVC
builder.Services.AddControllers();

// Configuración de Swagger para la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONFIGURACIÓN DE CORS ---
// Registra el servicio de CORS y define una política llamada "AllowAll"
// que permite peticiones desde cualquier origen, con cualquier método y cualquier cabecera.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Registra el DbContext para que la aplicación sepa cómo crearlo.
builder.Services.AddDbContext<TeamFinderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra SignalR para el chat en tiempo real
builder.Services.AddSignalR();

// Registra tus servicios personalizados
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IReputationService, ReputationService>();
builder.Services.AddScoped<IGameEventsService, GameEventsService>();

var app = builder.Build();

// 2. CONFIGURAR EL PIPELINE DE LA PETICIÓN HTTP

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- USAR EL MIDDLEWARE DE CORS ---
// Aplica la política "AllowAll" que definimos antes.
// ¡Importante! Esto debe ir antes de UseAuthorization y MapControllers.
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configuración del endpoint de SignalR
app.MapHub<ChatHub>("/chatHub");

app.Run();

public partial class Program { }