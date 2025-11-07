# CSharp Movable

A C# library that implements move semantics for `IDisposable` resources, inspired by Rust's ownership model. This library helps prevent double-disposal bugs and makes ownership transfer explicit.

## Features

- **Move Semantics**: Transfer ownership of resources explicitly, preventing accidental double-disposal
- **Compile-Time Safety**: Attempting to use a moved resource throws an exception
- **Generic Wrappers**: `Movable<T>` for any type and `MovableDisposable<T>` for `IDisposable` resources
- **Base Class**: `MovableDisposable` base class for creating custom movable disposable types
- **No-Op Disposal**: Moved resources are not disposed when the original wrapper is disposed

## Installation

```bash
dotnet add package Movable
```

## Quick Start

### Using Movable<T>

Wrap any value to add move semantics:

```csharp
using Movable;

var movable = new Movable<string>("Hello, World!");
Console.WriteLine(movable.Value); // "Hello, World!"

var value = movable.Move();
Console.WriteLine(value); // "Hello, World!"

// This will throw ObjectMovedException
// Console.WriteLine(movable.Value);
```

### Using MovableDisposable<T>

Wrap `IDisposable` resources with move semantics:

```csharp
using Movable;
using System.IO;

// Create a movable stream
using var movableStream = new MovableDisposable<MemoryStream>(new MemoryStream());

// Use the stream
movableStream.Resource.Write(new byte[] { 1, 2, 3 }, 0, 3);

// Transfer ownership
var stream = movableStream.Move();

// movableStream.Dispose() will now be a no-op
// The caller is responsible for disposing 'stream'
stream.Dispose();
```

### Inheriting from MovableDisposable

Create custom disposable types with built-in move semantics:

```csharp
using Movable;

public class FileHandle : MovableDisposable
{
    private readonly string _path;
    private FileStream? _stream;

    public FileHandle(string path)
    {
        _path = path;
        _stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public void Write(byte[] data)
    {
        ThrowIfMoved();
        ThrowIfDisposed();
        _stream?.Write(data, 0, data.Length);
    }

    protected override void DisposeManagedResources()
    {
        _stream?.Dispose();
        _stream = null;
    }
}

// Usage
var handle = new FileHandle("data.txt");
handle.Write(new byte[] { 1, 2, 3 });

var movedHandle = handle.Move();
// handle.Write(...) would throw ObjectMovedException

movedHandle.Dispose(); // Properly disposes the file stream
handle.Dispose(); // No-op, already moved
```

## Concepts

### Move Semantics

Move semantics allow you to transfer ownership of a resource from one variable to another. After a move:
- The original wrapper is marked as "moved"
- Attempting to access the moved wrapper throws `ObjectMovedException`
- The new owner is responsible for the resource's lifecycle

### Why Move Semantics?

Move semantics solve several common problems:

1. **Prevent Double Disposal**: After moving a resource, the original wrapper won't dispose it
2. **Explicit Ownership Transfer**: Makes it clear when ownership changes hands
3. **Resource Safety**: Compile-time prevention of use-after-move bugs

### Comparison with C++/Rust

While C# doesn't have true move semantics at the language level like C++ or Rust, this library provides a runtime-checked pattern that achieves similar goals:

- **C++/Rust**: Compile-time move checking
- **CSharp Movable**: Runtime move checking with clear exceptions

## API Reference

### IMovable<T>

```csharp
public interface IMovable<T>
{
    bool IsMoved { get; }
    T Move();
}
```

### Movable<T>

```csharp
public class Movable<T> : IMovable<T>
{
    public Movable(T value);
    public bool IsMoved { get; }
    public T Value { get; }
    public T Move();
    public bool TryMove(out T? value);
}
```

### MovableDisposable<T> where T : IDisposable

```csharp
public class MovableDisposable<T> : IDisposable, IMovable<T>
{
    public MovableDisposable(T resource);
    public bool IsMoved { get; }
    public bool IsDisposed { get; }
    public T Resource { get; }
    public T Move();
    public bool TryMove(out T? resource);
    public void Dispose();
}
```

### MovableDisposable

```csharp
public abstract class MovableDisposable : IDisposable, IMovable<MovableDisposable>
{
    public bool IsMoved { get; }
    public bool IsDisposed { get; }
    public MovableDisposable Move();
    public void Dispose();
    
    protected virtual void DisposeManagedResources();
    protected virtual void DisposeUnmanagedResources();
    protected void ThrowIfMoved();
    protected void ThrowIfDisposed();
}
```

## Exception Types

### ObjectMovedException

Thrown when attempting to access or move an object that has already been moved.

```csharp
var movable = new Movable<string>("test");
movable.Move();
movable.Move(); // Throws ObjectMovedException
```

## Building and Testing

```bash
# Build the library
dotnet build

# Run tests
dotnet test

# Build the entire solution
dotnet build CSharpMovable.sln
```

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Acknowledgments

This library is inspired by move semantics in Rust and C++, adapted for C#'s runtime environment.