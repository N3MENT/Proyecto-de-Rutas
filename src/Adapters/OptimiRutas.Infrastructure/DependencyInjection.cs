namespace OptimiRutas.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.Services;
using OptimiRutas.Infrastructure.Adapters;
using OptimiRutas.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IRutaRepository, InMemoryRutaRepository>();
        services.AddSingleton<IUsuarioRepository, InMemoryUsuarioRepository>();
        services.AddTransient<IOptimizadorRutaService, VecinoMasCercanoOptimizadorService>();

        return services;
    }
}
