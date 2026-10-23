using Godot;
using System;

using Zelynx.Patterns;
using Zelynx.UI.Windows;

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
    /// Deferred call to safely add manipulate the root scene tree
    /// </summary>
    private void Init()
    {
        // Startup Window Manager
        var windowManager = new WindowManager();
        GetTree().Root.AddChild(windowManager);

        var notify = windowManager.CreateWindow(WindowType.Notify);
    }
    

    public override void _ExitTree()
    {
        base._ExitTree();
    }
}