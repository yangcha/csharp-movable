namespace Movable;

/// <summary>
/// Base class for disposable resources that support move semantics.
/// </summary>
public abstract class MovableDisposable : IDisposable, IMovable<MovableDisposable>
{
    private bool _isMoved;
    private bool _isDisposed;

    /// <summary>
    /// Gets a value indicating whether this instance has been moved.
    /// </summary>
    public bool IsMoved => _isMoved;

    /// <summary>
    /// Gets a value indicating whether this instance has been disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Moves this instance, transferring ownership to the caller.
    /// After moving, this instance is marked as moved and should not be used.
    /// </summary>
    /// <returns>This instance.</returns>
    /// <exception cref="ObjectMovedException">Thrown when the instance has already been moved.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the instance has been disposed.</exception>
    public MovableDisposable Move()
    {
        ThrowIfMoved();
        ThrowIfDisposed();
        _isMoved = true;
        return this;
    }

    /// <summary>
    /// Disposes the instance if it hasn't been moved.
    /// If the instance has been moved, this is a no-op.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(); false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed || _isMoved)
        {
            return;
        }

        if (disposing)
        {
            DisposeManagedResources();
        }

        DisposeUnmanagedResources();
        _isDisposed = true;
    }

    /// <summary>
    /// Disposes managed resources. Override this to dispose of managed resources.
    /// </summary>
    protected virtual void DisposeManagedResources()
    {
    }

    /// <summary>
    /// Disposes unmanaged resources. Override this to dispose of unmanaged resources.
    /// </summary>
    protected virtual void DisposeUnmanagedResources()
    {
    }

    /// <summary>
    /// Ensures the instance has not been moved.
    /// </summary>
    /// <exception cref="ObjectMovedException">Thrown when the instance has been moved.</exception>
    protected void ThrowIfMoved()
    {
        if (_isMoved)
        {
            throw new ObjectMovedException($"The {GetType().Name} has been moved and can no longer be accessed.");
        }
    }

    /// <summary>
    /// Ensures the instance has not been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the instance has been disposed.</exception>
    protected void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~MovableDisposable()
    {
        Dispose(false);
    }
}
