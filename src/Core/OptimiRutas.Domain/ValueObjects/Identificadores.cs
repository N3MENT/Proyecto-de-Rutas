namespace OptimiRutas.Domain.ValueObjects;

public record RutaId(Guid Value)
{
    public static RutaId Nuevo() => new(Guid.NewGuid());
}

public record ParadaId(Guid Value)
{
    public static ParadaId Nuevo() => new(Guid.NewGuid());
}