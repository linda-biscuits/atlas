using System;
using System.Collections.Generic;

using Godot;

using Zelynx.Patterns;

namespace Zelynx.UI.Windows;

/// <summary>
/// Manages the creation, focus, and lifecycle of custom window instances within the UI.
/// </summary>
public partial class WindowManager : SingletonNode<WindowManager>
{
    private readonly List<Window> _activeWindows = new List<Window>();
    private CanvasLayer _canvas;
    private Control _rootControl;
    private const int CanvasLayerOrder = 10;

    public override void _EnterTree()
    {
        base._EnterTree();

        Name = "WindowManager";

        // Add canvas and root control and set to full rect
        _canvas = new CanvasLayer { Name = "GuiCanvas" , Layer = CanvasLayerOrder };
        _rootControl = new Control { Name = "GuiRoot" , MouseFilter = Control.MouseFilterEnum.Ignore };
        _rootControl.SetAnchorsPreset(Control.LayoutPreset.FullRect);

        _canvas.AddChild(_rootControl);
        AddChild(_canvas);

        // This is placed in the scene tree by the bootstrapper, so we don't need to add it manually.
    }


    public override void _ExitTree()
    {
        // Clean up all active windows
        foreach (var window in _activeWindows)
        {
            window.QueueFree();
        }
        
        _canvas.QueueFree();
        _rootControl.QueueFree();
        _activeWindows.Clear();

    }

    /// <summary>
    /// Spawns a custom content node inside a brand new window frame.
    /// </summary>
    public Window CreateWindow(WindowType windowType)
    {
        Window instance = (Window)WindowFactory.CreateWindow(windowType);

        // Defer in case the caller (e.g. another node's _Ready) is still inside tree setup
        Callable.From(() => FinishCreateWindow(instance)).CallDeferred();

        return instance;
    }

    private void FinishCreateWindow(Window windowInstance)
    {
        // Add to Autoload tree root first so _Ready() runs and populates node references
        _rootControl.AddChild(windowInstance);

        // Track and layer focus instantly
        _activeWindows.Add(windowInstance);
        
    }

    public void UnregisterWindow(Window window)
    {
        _activeWindows.Remove(window);
    }
}