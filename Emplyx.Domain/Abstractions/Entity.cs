namespace Emplyx.Domain.Abstractions;

/// <summary>
/// Minimal domain entity base that enforces identity-based equality.
/// </summary>
public abstract class Entity
{
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Entity id must be a non-empty GUID.", nameof(id));
        }

        Id = id;
    }

    protected Entity()
    {
    }

    public Guid Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }

    public static bool operator ==(Entity? left, Entity? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);

    public override int GetHashCode() => Id.GetHashCode();
}
