using Movable;
using System.IO;

Console.WriteLine("=== Move Semantics for IDisposable in C# - Examples ===\n");

// Example 1: Basic Movable<T>
Console.WriteLine("Example 1: Basic Movable<T>");
Console.WriteLine("---");
var movableString = new Movable<string>("Hello, World!");
Console.WriteLine($"Value: {movableString.Value}");
var movedString = movableString.Move();
Console.WriteLine($"Moved value: {movedString}");
Console.WriteLine($"Is moved: {movableString.IsMoved}");
try
{
    var _ = movableString.Value;
}
catch (ObjectMovedException e)
{
    Console.WriteLine($"Expected exception: {e.Message}");
}
Console.WriteLine();

// Example 2: MovableDisposable<T> with MemoryStream
Console.WriteLine("Example 2: MovableDisposable<T> with MemoryStream");
Console.WriteLine("---");
using var movableStream = new MovableDisposable<MemoryStream>(new MemoryStream());
movableStream.Resource.Write(new byte[] { 1, 2, 3, 4, 5 }, 0, 5);
Console.WriteLine($"Wrote 5 bytes to stream");
Console.WriteLine($"Stream position: {movableStream.Resource.Position}");

var stream = movableStream.Move();
Console.WriteLine($"Moved stream, position: {stream.Position}");
Console.WriteLine($"Original wrapper is moved: {movableStream.IsMoved}");

// movableStream.Dispose() will be called here but it's a no-op since it's moved
stream.Dispose(); // Properly dispose the moved stream
Console.WriteLine();

// Example 3: TryMove pattern
Console.WriteLine("Example 3: TryMove pattern");
Console.WriteLine("---");
var movableInt = new Movable<int>(42);
if (movableInt.TryMove(out var value))
{
    Console.WriteLine($"Successfully moved value: {value}");
}
if (!movableInt.TryMove(out var secondValue))
{
    Console.WriteLine("Second move attempt failed as expected");
}
Console.WriteLine();

// Example 4: Ownership transfer prevents double disposal
Console.WriteLine("Example 4: Ownership transfer prevents double disposal");
Console.WriteLine("---");
var stream1 = new MemoryStream();
Console.WriteLine("Created MemoryStream");

{
    var wrapper = new MovableDisposable<MemoryStream>(stream1);
    Console.WriteLine("Wrapped in MovableDisposable");
    
    var movedStream = wrapper.Move();
    Console.WriteLine("Moved ownership out of wrapper");
    
    // wrapper.Dispose() called here (using block exit) - but it's a no-op
}
Console.WriteLine("Wrapper disposed (no-op due to move)");

// We still have the stream and can use it
stream1.WriteByte(255);
Console.WriteLine($"Can still use the moved stream, length: {stream1.Length}");
stream1.Dispose();
Console.WriteLine("Manually disposed the moved stream");
Console.WriteLine();

Console.WriteLine("=== All examples completed successfully ===");
