namespace OptimiRutas.Application.Mappers;

using OptimiRutas.Application.DTOs;
using OptimiRutas.Domain.Entities;

public static class RutaMapper
{
    public static RutaDto MapearARutaDto(Ruta ruta)
    {
        var paradasDto = ruta.Paradas
            .OrderBy(p => p.Orden)
            .Select(p => new ParadaDto(
                p.Id.Value,
                p.Direccion.Calle,
                p.Direccion.Coordenada.Latitud,
                p.Direccion.Coordenada.Longitud,
                p.Completada,
                p.Orden))
            .ToList();

        var stats = ruta.ObtenerEstadisticas();
        var statsDto = new EstadisticasDto(stats.Total, stats.Completadas, stats.MinutosEstimados, stats.AhorroEstimado);

        return new RutaDto(
            ruta.Id.Value,
            ruta.Nombre,
            ruta.Repartidor,
            ruta.CalcularProgreso(),
            statsDto,
            paradasDto
        );
    }
}
