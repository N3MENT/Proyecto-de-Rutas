namespace OptimiRutas.Tests.Application;

using OptimiRutas.Application.Mappers;
using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;
using Xunit;

public class RutaMapperTests
{
    [Fact]
    public void MapearARutaDto_DebeMapearTodosLosCampos()
    {
        var ruta = new Ruta(RutaId.Nuevo(), "Ruta Centro", "Carlos");
        ruta.AgregarParada(new Direccion("Calle 1", new Coordenada(100, 200)));
        ruta.AgregarParada(new Direccion("Calle 2", new Coordenada(300, 400)));

        var dto = RutaMapper.MapearARutaDto(ruta);

        Assert.Equal(ruta.Id.Value, dto.Id);
        Assert.Equal("Ruta Centro", dto.Nombre);
        Assert.Equal("Carlos", dto.Repartidor);
        Assert.Equal(2, dto.Paradas.Count);
        Assert.Equal(0, dto.ProgresoPorcentaje);
        Assert.Equal(2, dto.Estadisticas.TotalParadas);
        Assert.Equal(0, dto.Estadisticas.Completadas);
    }

    [Fact]
    public void MapearARutaDto_ConParadaCompletada_DebeReflejarProgreso()
    {
        var ruta = new Ruta(RutaId.Nuevo(), "Ruta", "Repartidor");
        ruta.AgregarParada(new Direccion("Calle 1", new Coordenada(100, 200)));
        ruta.AgregarParada(new Direccion("Calle 2", new Coordenada(300, 400)));
        ruta.CompletarParada(ruta.Paradas.First().Id);

        var dto = RutaMapper.MapearARutaDto(ruta);

        Assert.Equal(50, dto.ProgresoPorcentaje);
        Assert.Equal(1, dto.Estadisticas.Completadas);
        Assert.True(dto.Paradas.First(p => p.Calle == "Calle 1").Completada);
        Assert.False(dto.Paradas.First(p => p.Calle == "Calle 2").Completada);
    }
}
