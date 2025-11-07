namespace Movable;

/// <summary>
/// A wrapper that provides move semantics for IDisposable resources.
/// </summary>
/// <typeparam name="T">The type of the disposable resource.</typeparam>
public class MovableDisposable<T> : IDisposable, IMovable<T> where T : IDisposable
{
    private T? _resource;
    private bool _isMoved;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovableDisposable{T}"/> class.
    /// </summary>
    /// <param name="resource">The disposable resource to wrap.</param>
    public MovableDisposable(T resource)
    {
        _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        _isMoved = false;
        _isDisposed = false;
    }

    /// <summary>
    /// Gets a value indicating whether this instance has been moved.
    /// </summary>
    public bool IsMoved => _isMoved;

    /// <summary>
    /// Gets a value indicating whether this instance has been disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Gets the wrapped resource, if not moved or disposed.
    /// </summary>
    /// <exception cref="ObjectMovedException">Thrown when the instance has been moved.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance has been disposed.</exception>
    public T Resource
    {
        get
        {
            ThrowIfMoved();
            ThrowIfDisposed();
            return _resource!;
        }
    }

    /// <summary>
    /// Moves the resource out of this instance, transferring ownership to the caller.
    /// The caller becomes responsible for disposing the resource.
    /// </summary>
    /// <returns>The moved resource.</returns>
    /// <exception cref="ObjectMovedException">Thrown when the instance has already been moved.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance has been disposed.</exception>
    public T Move()
    {
        ThrowIfMoved();
        ThrowIfDisposed();
        _isMoved = true;
        var resource = _resource!;
        _resource = default;
        return resource;
    }

    /// <summary>
    /// Attempts to move the resource out of this instance.
    /// </summary>
    /// <param name="resource">The moved resource, or default if already moved or disposed.</param>
    /// <returns>True if the move succeeded; false if already moved or disposed.</returns>
    public bool TryMove(out T? resource)
    {
        if (_isMoved || _isDisposed)
        {
            resource = default;
            return false;
        }

        _isMoved = true;
        resource = _resource;
        _resource = default;
        return true;
    }

    /// <summary>
    /// Disposes the wrapped resource if it hasn't been moved.
    /// If the resource has been moved, this is a no-op.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed || _isMoved)
        {
            return;
        }

        _resource?.Dispose();
        _resource = default;
        _isDisposed = true;
    }

    private void ThrowIfMoved()
    {
        if (_isMoved)
        {
            throw new ObjectMovedException($"The {typeof(T).Name} has been moved and can no longer be accessed.");
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }
}
