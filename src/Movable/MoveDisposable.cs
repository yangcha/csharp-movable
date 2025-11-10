namespace Movable
{

    public class MoveDisposable<TResource>(TResource resource) : IDisposable where TResource : class, IDisposable
    {
        private TResource? resource = resource ?? throw new ArgumentNullException(nameof(resource));

        public TResource Value => resource ?? throw new InvalidOperationException();

        public TResource Move()
        {
            TResource result = resource ?? throw new InvalidOperationException();
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
