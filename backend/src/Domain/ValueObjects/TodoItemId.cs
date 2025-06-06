namespace Domain.ValueObjects;

public class TodoItemId : IEquatable<TodoItemId>
{
    private TodoItemId(int value) => Value = value <= 0 
        ? throw new ArgumentException("El ID debe ser mayor que 0")
        : value;

    public int Value { get; }

    public static TodoItemId From(int value)
    {
        return new TodoItemId(value);
    }

    public bool Equals(TodoItemId? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
