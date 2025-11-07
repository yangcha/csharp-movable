namespace Movable;

/// <summary>
/// Represents an object that can be moved, transferring ownership of its resources.
/// </summary>
/// <typeparam name="T">The type of the movable object.</typeparam>
public interface IMovable<T>
{
    /// <summary>
    /// Gets a value indicating whether this instance has been moved.
    /// </summary>
    bool IsMoved { get; }

    /// <summary>
    /// Moves the value out of this instance, marking it as moved.
    /// </summary>
    /// <returns>The moved value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the instance has already been moved.</exception>
    T Move();
}
