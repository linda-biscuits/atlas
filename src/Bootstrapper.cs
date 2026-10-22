using Godot;
using System;

using Zelynx.Patterns;

/// <summary>
/// Only autoload, bootstraps the rest of the application.
/// </summary>
public partial class Bootstrapper : SingletonNode<Bootstrapper>
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
        GD.Print("Bootstrapper initialized!");
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
    }
}