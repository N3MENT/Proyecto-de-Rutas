namespace OptimiRutas.Tests.Domain;

using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;
using Xunit;

public class ParadaTests
{
    [Fact]
    public void NuevaParada_DebeTenerValoresCorrectos()
    {
        var id = ParadaId.Nuevo();
        var direccion = new Direccion("Calle Principal", new Coordenada(100, 200));

        var parada = new Parada(id, direccion, 0);

        Assert.Equal(id, parada.Id);
        Assert.Equal(direccion, parada.Direccion);
        Assert.False(parada.Completada);
        Assert.Equal(0, parada.Orden);
    }

    [Fact]
    public void NuevaParada_ConIdNulo_DebeLanzarExcepcion()
    {
        var direccion = new Direccion("Calle", new Coordenada(0, 0));

        Assert.Throws<ArgumentNullException>(() => new Parada(null!, direccion, 0));
    }

    [Fact]
    public void NuevaParada_ConDireccionNula_DebeLanzarExcepcion()
    {
        Assert.Throws<ArgumentNullException>(() => new Parada(ParadaId.Nuevo(), null!, 0));
    }

    [Fact]
    public void MarcarComoCompletada_DebeCambiarEstado()
    {
        var parada = new Parada(ParadaId.Nuevo(), new Direccion("Calle", new Coordenada(0, 0)), 0);

        parada.MarcarComoCompletada();

        Assert.True(parada.Completada);
    }

    [Fact]
    public void MarcarComoCompletada_YaCompletada_DebeLanzarExcepcion()
    {
        var parada = new Parada(ParadaId.Nuevo(), new Direccion("Calle", new Coordenada(0, 0)), 0);
        parada.MarcarComoCompletada();

        Assert.Throws<InvalidOperationException>(() => parada.MarcarComoCompletada());
    }

    [Fact]
    public void AsignarOrden_DebeCambiarValor()
    {
        var parada = new Parada(ParadaId.Nuevo(), new Direccion("Calle", new Coordenada(0, 0)), 0);

        parada.AsignarOrden(5);

        Assert.Equal(5, parada.Orden);
    }

    [Fact]
    public void AsignarOrden_Negativo_DebeLanzarExcepcion()
    {
        var parada = new Parada(ParadaId.Nuevo(), new Direccion("Calle", new Coordenada(0, 0)), 0);

        Assert.Throws<ArgumentException>(() => parada.AsignarOrden(-1));
    }
}
