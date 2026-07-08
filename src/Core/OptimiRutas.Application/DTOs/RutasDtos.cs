namespace OptimiRutas.Application.DTOs;

// --- DTOs DE ENTRADA (Comandos que expresan la intención del usuario) ---

public record CrearParadaDto(
    string Calle, 
    double Latitud, 
    double Longitud
);

public record RegistrarRutaCommand(
    string Nombre, 
    string Repartidor, 
    List<CrearParadaDto> Paradas
);

public record CompletarParadaCommand(
    Guid RutaId, 
    Guid ParadaId
);

// --- DTOs DE SALIDA (Lo que devolveremos a la pantalla o API) ---

public record ParadaDto(
    Guid Id, 
    string Calle, 
    double Lat, 
    double Lng, 
    bool Completada, 
    int Orden
);

public record EstadisticasDto(
    int TotalParadas, 
    int Completadas, 
    int MinutosEstimados, 
    double AhorroEstimado
);

public record RutaDto(
    Guid Id, 
    string Nombre, 
    string Repartidor, 
    int ProgresoPorcentaje, 
    EstadisticasDto Estadisticas, 
    List<ParadaDto> Paradas
);