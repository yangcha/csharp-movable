namespace Movable.Examples;

/// <summary>
/// Example implementation of a disposable file handle using MovableDisposable.
/// </summary>
public class FileHandle : MovableDisposable
{
    private readonly string _path;
    private FileStream? _stream;

    public FileHandle(string path)
    {
        _path = path;
        _stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public string Path
    {
        get
        {
            ThrowIfMoved();
            ThrowIfDisposed();
            return _path;
        }
    }

    public void Write(byte[] data)
    {
        ThrowIfMoved();
        ThrowIfDisposed();
        _stream?.Write(data, 0, data.Length);
    }

    public int Read(byte[] buffer)
    {
        ThrowIfMoved();
        ThrowIfDisposed();
        return _stream?.Read(buffer, 0, buffer.Length) ?? 0;
    }

    protected override void DisposeManagedResources()
    {
        _stream?.Dispose();
        _stream = null;
    }
}
