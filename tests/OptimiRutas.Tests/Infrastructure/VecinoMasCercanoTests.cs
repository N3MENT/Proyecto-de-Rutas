namespace OptimiRutas.Tests.Infrastructure;

using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;
using OptimiRutas.Infrastructure.Adapters;
using Xunit;

public class VecinoMasCercanoOptimizadorServiceTests
{
    private readonly VecinoMasCercanoOptimizadorService _optimizador = new();

    [Fact]
    public void Optimizar_VariasParadas_DebeRetornarOrdenadas()
    {
        var paradas = new List<Parada>
        {
            new(ParadaId.Nuevo(), new Direccion("Lejos", new Coordenada(500, 500)), 0),
            new(ParadaId.Nuevo(), new Direccion("Cerca", new Coordenada(10, 10)), 1),
            new(ParadaId.Nuevo(), new Direccion("Media", new Coordenada(250, 250)), 2),
        };
        var origen = new Coordenada(0, 0);

        var resultado = _optimizador.Optimizar(paradas, origen);

        Assert.Equal(3, resultado.Count);
        Assert.Equal("Cerca", resultado[0].Direccion.Calle);
        Assert.Equal("Media", resultado[1].Direccion.Calle);
        Assert.Equal("Lejos", resultado[2].Direccion.Calle);
    }

    [Fact]
    public void Optimizar_UnaParada_DebeRetornarEsaMisma()
    {
        var parada = new Parada(ParadaId.Nuevo(), new Direccion("Unica", new Coordenada(100, 200)), 0);
        var origen = new Coordenada(0, 0);

        var resultado = _optimizador.Optimizar(new[] { parada }, origen);

        Assert.Single(resultado);
        Assert.Equal("Unica", resultado[0].Direccion.Calle);
    }

    [Fact]
    public void Optimizar_ListaVacia_DebeRetornarVacia()
    {
        var resultado = _optimizador.Optimizar(new List<Parada>(), new Coordenada(0, 0));

        Assert.Empty(resultado);
    }
}
