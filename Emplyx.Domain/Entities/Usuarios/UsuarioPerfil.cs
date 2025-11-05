namespace Emplyx.Domain.Entities.Usuarios;

public sealed class UsuarioPerfil
{
    private UsuarioPerfil()
    {
    }

    private UsuarioPerfil(string? nombres, string? apellidos, string? departamento, string? cargo, string? telefono)
    {
        Nombres = nombres?.Trim();
        Apellidos = apellidos?.Trim();
        Departamento = departamento?.Trim();
        Cargo = cargo?.Trim();
        Telefono = telefono?.Trim();
    }

    public string? Nombres { get; private set; }

    public string? Apellidos { get; private set; }

    public string? Departamento { get; private set; }

    public string? Cargo { get; private set; }

    public string? Telefono { get; private set; }

    public static UsuarioPerfil Empty { get; } = new UsuarioPerfil();

    public static UsuarioPerfil Create(string? nombres, string? apellidos, string? departamento, string? cargo, string? telefono) =>
        new(nombres, apellidos, departamento, cargo, telefono);
}
