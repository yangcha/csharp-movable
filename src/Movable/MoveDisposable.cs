namespace Movable
{

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
