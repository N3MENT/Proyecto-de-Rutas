using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using OptimiRutas.Application.UseCases;
using OptimiRutas.Domain.Services;
using OptimiRutas.Domain.ValueObjects;
using OptimiRutas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Rate limiting - máximo 10 peticiones por minuto por IP
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });
});

// CORS restringido solo a dominios permitidos
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var latitud = builder.Configuration.GetValue<double>("OrigenNegocio:Latitud");
var longitud = builder.Configuration.GetValue<double>("OrigenNegocio:Longitud");
builder.Services.AddSingleton<Coordenada>(new Coordenada(latitud, longitud));

builder.Services.AddInfrastructure();

builder.Services.AddScoped<ObtenerTodasLasRutasUseCase>();
builder.Services.AddScoped<ObtenerRutaPorIdUseCase>();
builder.Services.AddScoped<RegistrarRutaUseCase>();
builder.Services.AddScoped<CompletarParadaUseCase>();
builder.Services.AddScoped<SugerirRutasUseCase>();
builder.Services.AddScoped<RegistrarUsuarioUseCase>();
builder.Services.AddScoped<IniciarSesionUseCase>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Rate limiting
app.UseRateLimiter();

// Headers de seguridad
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:;");
    context.Response.Headers.Append("Permissions-Policy", "geolocation=(), camera=(), microphone=()");
    await next();
});

app.UseCors("PermitirTodo");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

var ip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList
    .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "localhost";
Console.WriteLine($"Servidor corriendo en:");
Console.WriteLine($"  Local:     http://localhost:5052");
Console.WriteLine($"  Red local: http://{ip}:5052");
Console.WriteLine($"  Swagger:   http://localhost:5052/swagger");

app.Run();
