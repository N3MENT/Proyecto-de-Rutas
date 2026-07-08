namespace OptimiRutas.Domain.Entities;

using OptimiRutas.Domain.ValueObjects;

public class Parada
{
    public ParadaId Id { get; private set; } = null!;
    public Direccion Direccion { get; private set; } = null!;
    public bool Completada { get; private set; }
    public int Orden { get; private set; }

    // Constructor vacío protegido requerido por futuros ORM (como EF Core)
    protected Parada() { }

    public Parada(ParadaId id, Direccion direccion, int orden)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Direccion = direccion ?? throw new ArgumentNullException(nameof(direccion));
        Completada = false;
        Orden = orden;
    }

    // Comportamiento del negocio (Evitamos el "Modelo Anémico" sin simples sets públicos)
    public void MarcarComoCompletada()
    {
        if (Completada)
            throw new InvalidOperationException("La parada ya se encuentra marcada como completada.");
        
        Completada = true;
    }

    public void AsignarOrden(int nuevoOrden)
    {
        if (nuevoOrden < 0)
            throw new ArgumentException("El número de orden no puede ser negativo.", nameof(nuevoOrden));
            
        Orden = nuevoOrden;
    }
}