namespace Movable
{
    /// <summary>
    /// A wrapper that provides move semantics for IDisposable resources.
    /// </summary>
    /// <typeparam name="TResource">The type of the value to wrap.</typeparam>
    /// <param name="resource">Resource to wrap.</param>
    public sealed class MoveDisposable<TResource>(TResource resource) : IDisposable where TResource : class, IDisposable
    {
        private const string invalidMoveMessage = "Resource has already been moved.";
        private TResource? resource = resource ?? throw new ArgumentNullException(nameof(resource));

        public TResource Value => resource ?? throw new InvalidOperationException(invalidMoveMessage);

        public TResource Move()
        {
            TResource result = resource ?? throw new InvalidOperationException(invalidMoveMessage);
            resource = null;
            return result;
        }

        public void Dispose()
        {
            resource?.Dispose();
            resource = null;
        }
    }

}
