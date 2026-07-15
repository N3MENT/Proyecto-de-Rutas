namespace OptimiRutas.Domain.Entities;

using OptimiRutas.Domain.ValueObjects;

public class Usuario
{
    public UsuarioId Id { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string ContrasenaHash { get; private set; } = null!;

    protected Usuario() { }

    public Usuario(UsuarioId id, string nombre, string email, string contrasenaHash)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        ContrasenaHash = contrasenaHash ?? throw new ArgumentNullException(nameof(contrasenaHash));
    }
}
