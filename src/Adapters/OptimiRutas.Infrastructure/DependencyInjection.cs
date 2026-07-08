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
        // Cuando alguien pida IRutaRepository, entrégale una única instancia (Singleton) de InMemoryRutaRepository
        services.AddSingleton<IRutaRepository, InMemoryRutaRepository>();

        // Cuando el dominio pida IOptimizadorRutaService, entrégale una nueva instancia de VecinoMasCercanoOptimizadorService
        services.AddTransient<IOptimizadorRutaService, VecinoMasCercanoOptimizadorService>();

        return services;
    }
}