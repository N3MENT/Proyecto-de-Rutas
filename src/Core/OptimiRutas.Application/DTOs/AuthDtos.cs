namespace OptimiRutas.Application.DTOs;

public record RegistrarUsuarioCommand(
    string Nombre,
    string Email,
    string Contrasena
);

public record IniciarSesionCommand(
    string Email,
    string Contrasena
);

public record UsuarioDto(
    Guid Id,
    string Nombre,
    string Email
);

public enum TipoVehiculo
{
    Moto,
    Carro,
    Camioneta,
    Bicicleta,
    Caminando
}

public record SugerirRutaCommand(
    double OrigenLat,
    double OrigenLng,
    double DestinoLat,
    double DestinoLng,
    string? NombreDestino,
    string Vehiculo = "Carro",
    bool EvitarTrafico = false
);

public record OpcionRutaDto(
    Guid Id,
    string Nombre,
    int TotalParadas,
    int MinutosEstimados,
    double DistanciaKm,
    string Icono,
    List<ParadaDto> Paradas
);
