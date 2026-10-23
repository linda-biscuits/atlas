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
}

/// <summary>
/// Abstract window class to derive all other window types from.
/// </summary>
public abstract partial class Window : Control
{    
    public WindowState State { get; set; }
}