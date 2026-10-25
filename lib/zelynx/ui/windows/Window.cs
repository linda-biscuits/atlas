using Godot;
using System;

namespace Zelynx.UI.Windows;

/// <summary>
/// Window types, assists in setup and lookup.
/// </summary>
public enum WindowType{ Base, Debug, Inventory, CharacterProfile, Settings, Notify }

/// <summary>
/// Represents the state of a window, including its type, focus status, and instance reference.
/// </summary>
public class WindowState
{
    public WindowType Type { get; set; }
    public bool IsFocused { get; set; }
    public bool IsMaximized { get; set; } = false;
    public bool IsMinimized { get; set; } = false;
    public Vector2 PreviousPosition { get; set; }
    public Vector2 PreviousSize { get; set; }
    public Vector2 PreviousContentSize { get; set; }
}

/// <summary>
/// Abstract window class to derive all other window types from.
/// </summary>
public abstract partial class Window : Control
{    
    public WindowState State { get; set; }
    public void Close()
    {
        WindowManager.Instance.UnregisterWindow(this);
        QueueFree();
    }

    public void Maximize()
    {
        if(State.IsMaximized)
        {
            GlobalPosition = State.PreviousPosition;
            Size = State.PreviousSize;
            State.IsMaximized = false;
        }
        else
        {
            State.PreviousPosition = GlobalPosition;
            State.PreviousSize = Size;
            GlobalPosition = Vector2.Zero;
            Size = GetViewportRect().Size;
            State.IsMaximized = true;
        }
    }

    public void Minimize()
    {
        // Not yet implemented
    }
}