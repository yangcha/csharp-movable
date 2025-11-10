using Xunit;
using System.IO;

namespace Movable.Tests;

public class MovableDisposableTTests
{
    private class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    [Fact]
    public void MovableDisposableT_CanBeCreatedWithResource()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        
        Assert.False(movable.IsMoved);
        Assert.False(movable.IsDisposed);
        Assert.Same(resource, movable.Resource);
    }

    [Fact]
    public void MovableDisposableT_ConstructorThrowsOnNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MovableDisposable<TestDisposable>(null!));
    }

    [Fact]
    public void MovableDisposableT_MoveTransfersOwnership()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        
        var moved = movable.Move();
        
        Assert.Same(resource, moved);
        Assert.True(movable.IsMoved);
    }

    [Fact]
    public void MovableDisposableT_MovingTwiceThrows()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        movable.Move();
        
        Assert.Throws<ObjectMovedException>(() => movable.Move());
    }

    [Fact]
    public void MovableDisposableT_AccessingResourceAfterMoveThrows()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        movable.Move();
        
        Assert.Throws<ObjectMovedException>(() => movable.Resource);
    }

    [Fact]
    public void MovableDisposableT_DisposeDisposesResource()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        
        movable.Dispose();
        
        Assert.True(resource.IsDisposed);
        Assert.True(movable.IsDisposed);
    }

    [Fact]
    public void MovableDisposableT_MovedResourceIsNotDisposed()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        
        var moved = movable.Move();
        movable.Dispose();
        
        Assert.False(resource.IsDisposed);
        Assert.False(movable.IsDisposed);
    }

    [Fact]
    public void MovableDisposableT_TryMoveSucceedsWhenNotMoved()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        
        var success = movable.TryMove(out var moved);
        
        Assert.True(success);
        Assert.Same(resource, moved);
        Assert.True(movable.IsMoved);
    }

    [Fact]
    public void MovableDisposableT_TryMoveFailsWhenAlreadyMoved()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        movable.Move();
        
        var success = movable.TryMove(out var moved);
        
        Assert.False(success);
        Assert.Null(moved);
    }

    [Fact]
    public void MovableDisposableT_TryMoveFailsWhenDisposed()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        movable.Dispose();
        
        var success = movable.TryMove(out var moved);
        
        Assert.False(success);
        Assert.Null(moved);
    }

    [Fact]
    public void MovableDisposableT_AccessingResourceAfterDisposeThrows()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        movable.Dispose();
        
        Assert.Throws<ObjectDisposedException>(() => movable.Resource);
    }

    [Fact]
    public void MovableDisposableT_MovingAfterDisposeThrows()
    {
        var resource = new TestDisposable();
        var movable = new MovableDisposable<TestDisposable>(resource);
        movable.Dispose();
        
        Assert.Throws<ObjectDisposedException>(() => movable.Move());
    }

    [Fact]
    public void MovableDisposableT_WorksWithMemoryStream()
    {
        var stream = new MemoryStream();
        var movable = new MovableDisposable<MemoryStream>(stream);
        
        var moved = movable.Move();
        movable.Dispose();
        
        Assert.Same(stream, moved);
        Assert.True(movable.IsMoved);
        Assert.False(movable.IsDisposed);
        
        // Clean up the moved stream
        moved.Dispose();
    }

    [Fact]
    public void MovableDisposableT_UsingPatternDisposesIfNotMoved()
    {
        var resource = new TestDisposable();
        
        using (var movable = new MovableDisposable<TestDisposable>(resource))
        {
            Assert.False(resource.IsDisposed);
        }
        
        Assert.True(resource.IsDisposed);
    }

    [Fact]
    public void MovableDisposableT_UsingPatternDoesNotDisposeIfMoved()
    {
        var resource = new TestDisposable();
        TestDisposable? moved = null;
        
        using (var movable = new MovableDisposable<TestDisposable>(resource))
        {
            moved = movable.Move();
        }
        
        Assert.False(resource.IsDisposed);
        Assert.NotNull(moved);
        
        // Clean up the moved resource
        moved.Dispose();
        Assert.True(resource.IsDisposed);
    }
}
