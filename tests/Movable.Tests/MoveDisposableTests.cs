using Xunit;

namespace Movable.Tests;

public class MoveDisposableTests
{
    private class TestResource : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    [Fact]
    public void Constructor_WithValidResource_Succeeds()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);

        Assert.NotNull(moveDisposable);
    }

    [Fact]
    public void Constructor_WithNullResource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MoveDisposable<TestResource>(null!));
    }

    [Fact]
    public void Value_WhenResourceExists_ReturnsResource()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);

        var value = moveDisposable.Value;

        Assert.Same(resource, value);
    }

    [Fact]
    public void Value_AfterMove_ThrowsInvalidOperationException()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);
        moveDisposable.Move();

        Assert.Throws<InvalidOperationException>(() => moveDisposable.Value);
    }

    [Fact]
    public void Value_AfterDispose_ThrowsInvalidOperationException()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);
        moveDisposable.Dispose();

        Assert.Throws<InvalidOperationException>(() => moveDisposable.Value);
    }

    [Fact]
    public void Move_WhenResourceExists_ReturnsResourceAndSetsToNull()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);

        var movedResource = moveDisposable.Move();

        Assert.Same(resource, movedResource);
        Assert.Throws<InvalidOperationException>(() => moveDisposable.Value);
    }

    [Fact]
    public void Move_AfterAlreadyMoved_ThrowsInvalidOperationException()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);
        moveDisposable.Move();

        Assert.Throws<InvalidOperationException>(() => moveDisposable.Move());
    }

    [Fact]
    public void Move_AfterDispose_ThrowsInvalidOperationException()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);
        moveDisposable.Dispose();

        Assert.Throws<InvalidOperationException>(() => moveDisposable.Move());
    }

    [Fact]
    public void Dispose_WhenResourceExists_DisposesResource()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);

        moveDisposable.Dispose();

        Assert.True(resource.IsDisposed);
    }

    [Fact]
    public void Dispose_AfterMove_DoesNotDisposeMovedResource()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);
        var movedResource = moveDisposable.Move();

        moveDisposable.Dispose();

        Assert.False(movedResource.IsDisposed);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_IsSafe()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);

        moveDisposable.Dispose();
        moveDisposable.Dispose();

        Assert.True(resource.IsDisposed);
    }

    [Fact]
    public void Dispose_AfterMove_DoesNotThrow()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);
        moveDisposable.Move();

        var exception = Record.Exception(() => moveDisposable.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void Move_ReturnsOriginalResource_NotWrapper()
    {
        var resource = new TestResource();
        var moveDisposable = new MoveDisposable<TestResource>(resource);

        var movedResource = moveDisposable.Move();

        Assert.IsType<TestResource>(movedResource);
        Assert.Same(resource, movedResource);
    }
}