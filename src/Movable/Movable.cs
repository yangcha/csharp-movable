namespace Movable;

/// <summary>
/// A wrapper that provides move semantics for any value type or reference type.
/// </summary>
/// <typeparam name="T">The type of the value to wrap.</typeparam>
public class Movable<T> : IMovable<T>
{
    private T? _value;
    private bool _isMoved;

    /// <summary>
    /// Initializes a new instance of the <see cref="Movable{T}"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    public Movable(T value)
    {
        _value = value;
        _isMoved = false;
    }

    /// <summary>
    /// Gets a value indicating whether this instance has been moved.
    /// </summary>
    public bool IsMoved => _isMoved;

    /// <summary>
    /// Gets the value, if not moved.
    /// </summary>
    /// <exception cref="ObjectMovedException">Thrown when the instance has been moved.</exception>
    public T Value
    {
        get
        {
            ThrowIfMoved();
            return _value!;
        }
    }

    /// <summary>
    /// Moves the value out of this instance, marking it as moved.
    /// </summary>
    /// <returns>The moved value.</returns>
    /// <exception cref="ObjectMovedException">Thrown when the instance has already been moved.</exception>
    public T Move()
    {
        ThrowIfMoved();
        _isMoved = true;
        var value = _value!;
        _value = default;
        return value;
    }

    /// <summary>
    /// Attempts to move the value out of this instance.
    /// </summary>
    /// <param name="value">The moved value, or default if already moved.</param>
    /// <returns>True if the move succeeded; false if already moved.</returns>
    public bool TryMove(out T? value)
    {
        if (_isMoved)
        {
            value = default;
            return false;
        }

        _isMoved = true;
        value = _value;
        _value = default;
        return true;
    }

    private void ThrowIfMoved()
    {
        if (_isMoved)
        {
            throw new ObjectMovedException($"The {typeof(T).Name} has been moved and can no longer be accessed.");
        }
    }
}
