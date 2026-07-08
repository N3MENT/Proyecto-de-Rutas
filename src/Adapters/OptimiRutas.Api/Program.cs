using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models; // ESTE es el que necesitamos
using OptimiRutas.Application.UseCases;
using OptimiRutas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar CORS (Para permitir que tu index.html se conecte sin errores de seguridad)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Conectar la Infraestructura (Hexágono Exterior - Adaptadores)
builder.Services.AddInfrastructure();

// 3. Conectar la Aplicación (Hexágono Interior - Casos de Uso)
builder.Services.AddScoped<RegistrarRutaUseCase>();
builder.Services.AddScoped<CompletarParadaUseCase>();

// 4. Configurar Controladores y Swagger (Presentación)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirTodo");
app.UseAuthorization();
app.MapControllers();
app.Run();