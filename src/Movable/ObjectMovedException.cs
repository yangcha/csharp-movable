namespace Movable;

/// <summary>
/// Exception thrown when an operation is attempted on a moved object.
/// </summary>
public class ObjectMovedException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectMovedException"/> class.
    /// </summary>
    public ObjectMovedException()
        : base("Cannot access an object that has been moved.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectMovedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ObjectMovedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectMovedException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ObjectMovedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
