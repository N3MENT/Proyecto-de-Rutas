namespace OptimiRutas.Infrastructure.Adapters;

using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.Services;
using OptimiRutas.Domain.ValueObjects;

public class VecinoMasCercanoOptimizadorService : IOptimizadorRutaService
{
    public IReadOnlyList<Parada> Optimizar(IEnumerable<Parada> paradas, Coordenada puntoInicio)
    {
        var desordenadas = paradas.ToList();
        var ordenadas = new List<Parada>();
        var coordenadaActual = puntoInicio;

        while (desordenadas.Count > 0)
        {
            // Buscamos la parada cuya distancia sea la mínima respecto al punto actual
            var masCercana = desordenadas
                .OrderBy(p => CalcularDistanciaEuclidiana(coordenadaActual, p.Direccion.Coordenada))
                .First();

            // Avanzamos nuestro "vehículo virtual" hacia esa parada
            coordenadaActual = masCercana.Direccion.Coordenada;
            ordenadas.Add(masCercana);
            desordenadas.Remove(masCercana);
        }

        return ordenadas.AsReadOnly();
    }

    private static double CalcularDistanciaEuclidiana(Coordenada origen, Coordenada destino)
    {
        // Fórmula del teorema de Pitágoras para distancia entre dos puntos cartesianos:
        // d = sqrt((x2 - x1)^2 + (y2 - y1)^2)
        var diferenciaLatitud = destino.Latitud - origen.Latitud;
        var diferenciaLongitud = destino.Longitud - origen.Longitud;
        
        return Math.Sqrt(Math.Pow(diferenciaLatitud, 2) + Math.Pow(diferenciaLongitud, 2));
    }
}