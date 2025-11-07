using Xunit;

namespace Movable.Tests;

public class MovableTests
{
    [Fact]
    public void Movable_CanBeCreatedWithValue()
    {
        var movable = new Movable<string>("test");
        Assert.False(movable.IsMoved);
        Assert.Equal("test", movable.Value);
    }

    [Fact]
    public void Movable_ValueCanBeAccessed()
    {
        var movable = new Movable<int>(42);
        Assert.Equal(42, movable.Value);
    }

    [Fact]
    public void Movable_MoveTransfersValue()
    {
        var movable = new Movable<string>("test");
        var value = movable.Move();
        
        Assert.Equal("test", value);
        Assert.True(movable.IsMoved);
    }

    [Fact]
    public void Movable_AccessingValueAfterMoveThrows()
    {
        var movable = new Movable<string>("test");
        movable.Move();
        
        Assert.Throws<ObjectMovedException>(() => movable.Value);
    }

    [Fact]
    public void Movable_MovingTwiceThrows()
    {
        var movable = new Movable<string>("test");
        movable.Move();
        
        Assert.Throws<ObjectMovedException>(() => movable.Move());
    }

    [Fact]
    public void Movable_TryMoveSucceedsWhenNotMoved()
    {
        var movable = new Movable<string>("test");
        var success = movable.TryMove(out var value);
        
        Assert.True(success);
        Assert.Equal("test", value);
        Assert.True(movable.IsMoved);
    }

    [Fact]
    public void Movable_TryMoveFailsWhenAlreadyMoved()
    {
        var movable = new Movable<string>("test");
        movable.Move();
        var success = movable.TryMove(out var value);
        
        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void Movable_SupportsReferenceTypes()
    {
        var obj = new object();
        var movable = new Movable<object>(obj);
        var moved = movable.Move();
        
        Assert.Same(obj, moved);
    }

    [Fact]
    public void Movable_SupportsValueTypes()
    {
        var movable = new Movable<int>(123);
        var moved = movable.Move();
        
        Assert.Equal(123, moved);
    }
}
