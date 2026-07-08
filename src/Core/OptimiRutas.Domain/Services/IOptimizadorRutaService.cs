namespace OptimiRutas.Domain.Services;

using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;

// Interfaz pura. La implementación técnica real vivirá en la capa de Infraestructura.
public interface IOptimizadorRutaService
{
    IReadOnlyList<Parada> Optimizar(IEnumerable<Parada> paradas, Coordenada puntoInicio);
}