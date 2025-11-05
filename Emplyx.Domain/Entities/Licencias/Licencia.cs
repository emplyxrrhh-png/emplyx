using Emplyx.Domain.Abstractions;

namespace Emplyx.Domain.Entities.Licencias;

public sealed class Licencia : Entity, IAggregateRoot
{
    private readonly HashSet<LicenciaModulo> _modulos = new();

    private Licencia()
    {
    }

    public Licencia(
        Guid id,
        string codigo,
        string nombre,
        DateTime inicioVigenciaUtc,
        DateTime? finVigenciaUtc = null,
        int? limiteUsuarios = null,
        bool esTrial = false)
        : base(id)
    {
        Codigo = string.IsNullOrWhiteSpace(codigo)
            ? throw new ArgumentException("License code cannot be empty.", nameof(codigo))
            : codigo.Trim().ToUpperInvariant();

        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("License name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        InicioVigenciaUtc = inicioVigenciaUtc;
        FinVigenciaUtc = finVigenciaUtc;
        LimiteUsuarios = limiteUsuarios;
        EsTrial = esTrial;
        IsRevoked = false;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public string Codigo { get; private set; } = string.Empty;

    public string Nombre { get; private set; } = string.Empty;

    public DateTime InicioVigenciaUtc { get; private set; }

    public DateTime? FinVigenciaUtc { get; private set; }

    public int? LimiteUsuarios { get; private set; }

    public bool EsTrial { get; private set; }

    public bool IsRevoked { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<LicenciaModulo> Modulos => _modulos;

    public void Revoke()
    {
        if (IsRevoked)
        {
            return;
        }

        IsRevoked = true;
        Touch();
    }

    public void Restore()
    {
        if (!IsRevoked)
        {
            return;
        }

        IsRevoked = false;
        Touch();
    }

    public void UpdateVigencia(DateTime inicio, DateTime? fin)
    {
        InicioVigenciaUtc = inicio;
        FinVigenciaUtc = fin;
        Touch();
    }

    public void UpdateLimiteUsuarios(int? limiteUsuarios)
    {
        if (limiteUsuarios is < 0)
        {
            throw new ArgumentException("User limit must be greater or equal to zero.", nameof(limiteUsuarios));
        }

        LimiteUsuarios = limiteUsuarios;
        Touch();
    }

    public void UpdateMetadata(string nombre, bool esTrial)
    {
        Nombre = string.IsNullOrWhiteSpace(nombre)
            ? throw new ArgumentException("License name cannot be empty.", nameof(nombre))
            : nombre.Trim();

        EsTrial = esTrial;
        Touch();
    }

    public void AttachModulo(Guid moduloId)
    {
        if (moduloId == Guid.Empty)
        {
            throw new ArgumentException("Module id must be provided.", nameof(moduloId));
        }

        if (_modulos.Any(m => m.ModuloId == moduloId))
        {
            return;
        }

        _modulos.Add(new LicenciaModulo(Id, moduloId, DateTime.UtcNow));
        Touch();
    }

    public void DetachModulo(Guid moduloId)
    {
        var modulo = _modulos.SingleOrDefault(m => m.ModuloId == moduloId);
        if (modulo is null)
        {
            return;
        }

        _modulos.Remove(modulo);
        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
