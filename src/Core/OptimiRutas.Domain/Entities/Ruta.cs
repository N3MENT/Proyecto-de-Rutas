namespace OptimiRutas.Domain.Entities;

using OptimiRutas.Domain.ValueObjects;
using OptimiRutas.Domain.Services;

public class Ruta
{
    public RutaId Id { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string Repartidor { get; private set; } = null!;
    
    // Encapsulación de colección: Exponemos solo de lectura hacia afuera
    private readonly List<Parada> _paradas = new();
    public IReadOnlyCollection<Parada> Paradas => _paradas.AsReadOnly();

    protected Ruta() { }

    public Ruta(RutaId id, string nombre, string repartidor)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la ruta es obligatorio.", nameof(nombre));
        if (string.IsNullOrWhiteSpace(repartidor))
            throw new ArgumentException("El nombre del repartidor es obligatorio.", nameof(repartidor));
            
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Nombre = nombre;
        Repartidor = repartidor;
    }

    // --- MÉTODOS DE NEGOCIO (INVARIANTES) ---

    public void AgregarParada(Direccion direccion)
    {
        var nuevoOrden = _paradas.Count;
        var nuevaParada = new Parada(ParadaId.Nuevo(), direccion, nuevoOrden);
        _paradas.Add(nuevaParada);
    }

    public void CompletarParada(ParadaId paradaId)
    {
        var parada = _paradas.FirstOrDefault(p => p.Id == paradaId);
        if (parada is null)
            throw new InvalidOperationException("La parada especificada no pertenece a esta ruta.");

        parada.MarcarComoCompletada();
    }

    public void OptimizarRecorrido(IOptimizadorRutaService optimizador, Coordenada puntoInicio)
    {
        if (_paradas.Count <= 1) return;

        // Le pedimos al servicio de dominio que calcule el mejor orden
        var paradasOrdenadas = optimizador.Optimizar(_paradas, puntoInicio);
        
        _paradas.Clear();
        for (int i = 0; i < paradasOrdenadas.Count; i++)
        {
            paradasOrdenadas[i].AsignarOrden(i);
            _paradas.Add(paradasOrdenadas[i]);
        }
    }

    public int CalcularProgreso()
    {
        if (_paradas.Count == 0) return 0;
        var completadas = _paradas.Count(p => p.Completada);
        return (int)Math.Round((double)completadas / _paradas.Count * 100);
    }

    public (int Total, int Completadas, int MinutosEstimados, double AhorroEstimado) ObtenerEstadisticas()
    {
        var total = _paradas.Count;
        var completadas = _paradas.Count(p => p.Completada);
        var minutos = total * 15; // Regla del negocio: 15 min por parada
        var ahorro = total * 6.10; // Regla del negocio: cálculo de ahorro en combustible
        
        return (total, completadas, minutos, ahorro);
    }
}