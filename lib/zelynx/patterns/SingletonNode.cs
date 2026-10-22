using System;
using System.Threading;

using Godot;

namespace Zelynx.Patterns;

/* <summary>
Abstract singleton class that uses Lazy initialization for thread safety.
</summary>
<example>
using Zelynx.Patterns;

public partial class YourClass : SingletonNode<YourClass>
{
    public override void _EnterTree()
    {
        base._EnterTree();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
    }
}
</example>
*/

public abstract partial class SingletonNode<T> : Node where T : SingletonNode<T>
{
    public static T Instance
    {
        get
        {
            if(_instance == null){ throw new InvalidOperationException($"Singleton {typeof(T)} is not initialized before use!"); }
            return _instance.Value;
        }
    }
    private static Lazy<T> _instance;

    public override void _EnterTree()
    {
        var candidate = new Lazy<T>(() => (T)this, LazyThreadSafetyMode.ExecutionAndPublication);
        var existing = Interlocked.CompareExchange(ref _instance, candidate, null);

        if (existing != null && existing.Value != this)
        {
            GD.PushError($"Singleton {typeof(T)} is already initialized with a different instance.");
            QueueFree();
            return;
            
        }
    }

    public override void _ExitTree()
    {
        var current = _instance;
        if (current != null && current.IsValueCreated && current.Value == this)
        {
            Interlocked.CompareExchange(ref _instance, null, current);
        }
    }
}