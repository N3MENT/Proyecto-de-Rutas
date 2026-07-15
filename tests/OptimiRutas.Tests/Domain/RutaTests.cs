namespace OptimiRutas.Tests.Domain;

using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;
using Xunit;

public class RutaTests
{
    private static Ruta CrearRutaConParadas(int cantidad)
    {
        var ruta = new Ruta(RutaId.Nuevo(), "Ruta Test", "Repartidor");
        for (int i = 0; i < cantidad; i++)
        {
            ruta.AgregarParada(new Direccion($"Calle {i}", new Coordenada(i * 100, i * 50)));
        }
        return ruta;
    }

    [Fact]
    public void NuevaRuta_DebeTenerDatosCorrectos()
    {
        var id = RutaId.Nuevo();
        var ruta = new Ruta(id, "Ruta Centro", "Carlos");

        Assert.Equal(id, ruta.Id);
        Assert.Equal("Ruta Centro", ruta.Nombre);
        Assert.Equal("Carlos", ruta.Repartidor);
        Assert.Empty(ruta.Paradas);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void NuevaRuta_ConNombreInvalido_DebeLanzarExcepcion(string? nombre)
    {
        Assert.Throws<ArgumentException>(() => new Ruta(RutaId.Nuevo(), nombre!, "Carlos"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void NuevaRuta_ConRepartidorInvalido_DebeLanzarExcepcion(string? repartidor)
    {
        Assert.Throws<ArgumentException>(() => new Ruta(RutaId.Nuevo(), "Ruta", repartidor!));
    }

    [Fact]
    public void NuevaRuta_ConIdNulo_DebeLanzarExcepcion()
    {
        Assert.Throws<ArgumentNullException>(() => new Ruta(null!, "Ruta", "Carlos"));
    }

    [Fact]
    public void AgregarParada_DebeIncrementarOrden()
    {
        var ruta = CrearRutaConParadas(3);

        Assert.Equal(3, ruta.Paradas.Count);
        Assert.Equal(0, ruta.Paradas.ElementAt(0).Orden);
        Assert.Equal(1, ruta.Paradas.ElementAt(1).Orden);
        Assert.Equal(2, ruta.Paradas.ElementAt(2).Orden);
    }

    [Fact]
    public void CompletarParada_DebeMarcarComoCompletada()
    {
        var ruta = CrearRutaConParadas(2);
        var paradaId = ruta.Paradas.First().Id;

        ruta.CompletarParada(paradaId);

        Assert.True(ruta.Paradas.First(p => p.Id == paradaId).Completada);
    }

    [Fact]
    public void CompletarParada_ParadaNoExistente_DebeLanzarExcepcion()
    {
        var ruta = CrearRutaConParadas(2);

        Assert.Throws<InvalidOperationException>(() => ruta.CompletarParada(ParadaId.Nuevo()));
    }

    [Fact]
    public void CompletarParada_YaCompletada_DebeLanzarExcepcion()
    {
        var ruta = CrearRutaConParadas(1);
        var paradaId = ruta.Paradas.First().Id;
        ruta.CompletarParada(paradaId);

        Assert.Throws<InvalidOperationException>(() => ruta.CompletarParada(paradaId));
    }

    [Fact]
    public void CalcularProgreso_SinParadas_DebeRetornarCero()
    {
        var ruta = new Ruta(RutaId.Nuevo(), "Ruta", "Carlos");

        Assert.Equal(0, ruta.CalcularProgreso());
    }

    [Fact]
    public void CalcularProgreso_ConParadasParciales_DebeCalcularCorrectamente()
    {
        var ruta = CrearRutaConParadas(4);
        ruta.CompletarParada(ruta.Paradas.ElementAt(0).Id);
        ruta.CompletarParada(ruta.Paradas.ElementAt(1).Id);

        Assert.Equal(50, ruta.CalcularProgreso());
    }

    [Fact]
    public void ObtenerEstadisticas_DebeCalcularCorrectamente()
    {
        var ruta = CrearRutaConParadas(3);
        ruta.CompletarParada(ruta.Paradas.First().Id);

        var stats = ruta.ObtenerEstadisticas();

        Assert.Equal(3, stats.Total);
        Assert.Equal(1, stats.Completadas);
        Assert.Equal(45, stats.MinutosEstimados);
        Assert.Equal(18.30, stats.AhorroEstimado, 2);
    }

    [Fact]
    public void Paradas_DebeSerSoloLectura()
    {
        var ruta = CrearRutaConParadas(1);

        Assert.IsAssignableFrom<IReadOnlyCollection<Parada>>(ruta.Paradas);
    }
}
