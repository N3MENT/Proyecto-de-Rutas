namespace OptimiRutas.Infrastructure.Persistence;

using System.Collections.Concurrent;
using OptimiRutas.Application.Ports;
using OptimiRutas.Domain.Entities;
using OptimiRutas.Domain.ValueObjects;

public class InMemoryRutaRepository : IRutaRepository
{
    // Nuestra "Base de Datos" temporal en la memoria RAM del servidor
    private readonly ConcurrentDictionary<Guid, Ruta> _rutas = new();

    public InMemoryRutaRepository()
    {
        // Podemos inyectar una ruta de prueba (Semilla / Seed Data) para tener algo al iniciar
        CargarDatosSemilla();
    }

    public Task<Ruta?> ObtenerPorIdAsync(RutaId id, CancellationToken cancellationToken = default)
    {
        _rutas.TryGetValue(id.Value, out var ruta);
        return Task.FromResult(ruta);
    }

    public Task<IEnumerable<Ruta>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_rutas.Values.AsEnumerable());
    }

    public Task GuardarAsync(Ruta ruta, CancellationToken cancellationToken = default)
    {
        _rutas[ruta.Id.Value] = ruta;
        return Task.CompletedTask;
    }

    public Task ActualizarAsync(Ruta ruta, CancellationToken cancellationToken = default)
    {
        _rutas[ruta.Id.Value] = ruta;
        return Task.CompletedTask;
    }

    private void CargarDatosSemilla()
    {
        var idPrueba = new RutaId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var rutaPrueba = new Ruta(idPrueba, "Ruta Matutina Centro", "Carlos Gómez");
        
        rutaPrueba.AgregarParada(new Direccion("Av. Francisco de Miranda 124 (Supermercado)", new Coordenada(10.4918, -66.8553)));
        rutaPrueba.AgregarParada(new Direccion("Calle de la Paz 89 (Panadería)", new Coordenada(10.4863, -66.9029)));
        rutaPrueba.AgregarParada(new Direccion("Plaza Venezuela Edificio C", new Coordenada(10.4924, -66.8834)));

        _rutas[idPrueba.Value] = rutaPrueba;
    }
}