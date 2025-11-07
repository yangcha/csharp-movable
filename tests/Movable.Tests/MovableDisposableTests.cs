using Xunit;

namespace Movable.Tests;

public class MovableDisposableTests
{
    private class TestResource : MovableDisposable
    {
        public bool ManagedDisposed { get; private set; }
        public bool UnmanagedDisposed { get; private set; }

        protected override void DisposeManagedResources()
        {
            ManagedDisposed = true;
        }

        protected override void DisposeUnmanagedResources()
        {
            UnmanagedDisposed = true;
        }

        public void DoSomething()
        {
            ThrowIfMoved();
            ThrowIfDisposed();
        }
    }

    [Fact]
    public void MovableDisposable_InitiallyNotMovedOrDisposed()
    {
        var resource = new TestResource();
        Assert.False(resource.IsMoved);
        Assert.False(resource.IsDisposed);
    }

    [Fact]
    public void MovableDisposable_CanBeMoved()
    {
        var resource = new TestResource();
        var moved = resource.Move();
        
        Assert.Same(resource, moved);
        Assert.True(resource.IsMoved);
    }

    [Fact]
    public void MovableDisposable_MovingTwiceThrows()
    {
        var resource = new TestResource();
        resource.Move();
        
        Assert.Throws<ObjectMovedException>(() => resource.Move());
    }

    [Fact]
    public void MovableDisposable_CanBeDisposed()
    {
        var resource = new TestResource();
        resource.Dispose();
        
        Assert.True(resource.IsDisposed);
        Assert.True(resource.ManagedDisposed);
        Assert.True(resource.UnmanagedDisposed);
    }

    [Fact]
    public void MovableDisposable_MovedResourcesAreNotDisposed()
    {
        var resource = new TestResource();
        resource.Move();
        resource.Dispose();
        
        Assert.False(resource.IsDisposed);
        Assert.False(resource.ManagedDisposed);
        Assert.False(resource.UnmanagedDisposed);
    }

    [Fact]
    public void MovableDisposable_DisposingTwiceIsNoOp()
    {
        var resource = new TestResource();
        resource.Dispose();
        resource.Dispose();
        
        Assert.True(resource.IsDisposed);
    }

    [Fact]
    public void MovableDisposable_MovingDisposedResourceThrows()
    {
        var resource = new TestResource();
        resource.Dispose();
        
        Assert.Throws<ObjectDisposedException>(() => resource.Move());
    }

    [Fact]
    public void MovableDisposable_AccessingMovedResourceThrows()
    {
        var resource = new TestResource();
        resource.Move();
        
        Assert.Throws<ObjectMovedException>(() => resource.DoSomething());
    }

    [Fact]
    public void MovableDisposable_AccessingDisposedResourceThrows()
    {
        var resource = new TestResource();
        resource.Dispose();
        
        Assert.Throws<ObjectDisposedException>(() => resource.DoSomething());
    }
}
