using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Godot;

using GdUnit4;
using static GdUnit4.Assertions;

using Zelynx.Patterns;

/// <summary>
/// Only autoload, bootstraps the rest of the application.
/// </summary>
public partial class SingletonNodeMock : SingletonNode<SingletonNodeMock>
{
    public override void _EnterTree()
    {
        base._EnterTree();
        CallDeferred("Init");
    }

    /// <summary>
    /// Deferred call to safely add nodes, since we will be adding them to our root node and not oursives/this.
    /// </summary>
    private void Init()
    {
        GD.Print("SingletonNodeMock initialized.");
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
    }
}

[TestSuite]
[RequireGodotRuntime]
public class SingletonNodeTests
{
    private SingletonNodeMock _mockNode;
    private const int threads = 32;

    [After]
    public void Cleanup()
    {
        _mockNode?._ExitTree();
        _mockNode?.Free();
        _mockNode = null;
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task ConcurrentAccess()
    {
        // Generate mock node and simulate it being in the scene tree
        _mockNode = new SingletonNodeMock();
        _mockNode._EnterTree();

        // Prepare a concurrent bag to collect instance values given to multiple threads
        var results = new ConcurrentBag<SingletonNodeMock>();
        using var barrier = new Barrier(threads);

        await Task.WhenAll(Enumerable.Range(0, threads).Select(_ => Task.Run(() =>
        {
            barrier.SignalAndWait();
            results.Add(SingletonNodeMock.Instance);
        })));

        // Ensure all instances are the same and only one ref.
        AssertThat(results.Count).IsEqual(threads);
        AssertThat(results.Distinct().Count()).IsEqual(1);
        AssertThat(results.First()).IsSame(_mockNode);
    }

    [TestCase]
    [RequireGodotRuntime]
    /// Run entertree from multiple threads
    public async Task ConcurrentRegistration()
    {
        // Create 32 threads and 32 singleton mock nodes
        var nodes = Enumerable.Range(0, threads).Select(_ => new SingletonNodeMock()).ToArray();
        using var barrier = new Barrier(threads);

        // Run entertree to simulate many concurrent instantiations
        await Task.WhenAll(nodes.Select(n => Task.Run(() =>
        {
            barrier.SignalAndWait();
            n._EnterTree();
        })));

        // Verify that only one instance was registered as the singleton ref
        var instance = SingletonNodeMock.Instance;
        AssertThat(nodes.Count(n => ReferenceEquals(n, instance))).IsEqual(1);
        
        // Clear if needed
        foreach (var n in nodes.Where(n => n != _mockNode))
        {
            if(n != null)
            {
                n._ExitTree();
                n.Free();
            }
        }
    }

    [TestCase]
    [RequireGodotRuntime]
    /// Basic single thread test that makes sure exception occurs on illegal access
    public void Instance_BeforeInit_Throws()
    {
        // Generate mock node and simulate it being in the scene tree
        _mockNode = new SingletonNodeMock();

        AssertThrown(() => _ = SingletonNodeMock.Instance)
            .IsInstanceOf<System.InvalidOperationException>();
    }
}

