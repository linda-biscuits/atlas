using Godot;
using System;
using System.Collections.Generic;

namespace Zelynx.UI.Windows;

/// <summary>
/// Handles window creation and default setup. Todo: Implement Lazy Loading singleton.
/// </summary>
public static class WindowFactory
{
    /// <summary>
    /// Base path that points to window prefabs. Todo: Append from a base zelynx library path.
    /// </summary>
    public const string WindowPrefabPath = "res://lib/zelynx/ui/prefabs";

    /// <summary>
    /// Holds all possible window types and their corresponding prefab paths.
    /// </summary>
    private static readonly Dictionary<WindowType, string> WindowPaths = new()
    {
        { WindowType.Settings, WindowPrefabPath + "/Settings.tscn" },
        { WindowType.Inventory, WindowPrefabPath + "/PlayerInventory.tscn" },
        { WindowType.CharacterProfile, WindowPrefabPath + "/PlayerProfile.tscn" },
        { WindowType.Notify, WindowPrefabPath + "/NotifyWindow.tscn" }
    };

    /// <summary>
    /// Creates a window of the specified type.
    /// </summary>
    /// <param name="type">The type of window to create. Must be one of the registered window types.</param>
    /// <returns>The instantiated window node, or null if loading fails.</returns>
    public static Control CreateWindow(WindowType type)
    {
        // Throw if we don't have a requested type
        if (!WindowPaths.TryGetValue(type, out string path))
        {
            throw new ArgumentException($"Window type {type} has no registered scene path.");
        }

        // Safely load and instantiate the pre-baked window scene
        var scene = GD.Load<PackedScene>(path);
        if (scene == null)
        {
            GD.PrintErr($"Failed to load window scene at: {path}");
            return null;
        }

        // Instantiate from defaults but do not attach
        return scene.Instantiate<Control>();
    }
}