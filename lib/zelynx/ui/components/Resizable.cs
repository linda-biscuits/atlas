using System.Collections.Generic;

using Godot;

namespace Zelynx.UI.Windows;

/// <summary>
/// A component that allows the parent window to be resized using the bottom right corner. User must populate ResizeTarget with the window to be resized.
/// </summary>
[GlobalClass, Tool, Icon("res://lib/zelynx/icons/draggable_16x16.svg")]
public partial class Resizable: Node
{
    [Export] public Control ResizeTarget { get; set; } 
    [Export] public int GrabSize { get; set; } = 10;

    private bool _isResizing;
    private Vector2 _startSize;
    private Vector2 _startMouse;


    /// <summary>
    /// Initializes the draggable component and sets up the drag handle input events.
    /// </summary>
    public override void _Ready()
    {
        SetProcessInput(false);
        
        if(Engine.IsEditorHint()){ return; }
        if(ResizeTarget == null){ GD.PrintErr("ResizeTarget is not assigned."); return; }

        ResizeTarget.GuiInput += OnTargetGuiInput;
    }

    public override void _ExitTree() => _isResizing = false;

    public void OnTargetGuiInput(InputEvent e)
    {
        if(e is InputEventMouseMotion mouseMotion && !_isResizing){
            _enableResizingMouseCursor(InCorner(mouseMotion.Position)); // Change mouse cursor if we are above the resize corner
        } 
        else if( e is InputEventMouseButton{ ButtonIndex: MouseButton.Left, Pressed: true } b && InCorner(b.Position))
        {
            // Start resizing and handoff to _Input for processing mouse motion events
            _isResizing = true;
            _startSize = ResizeTarget.Size;
            _startMouse = ResizeTarget.GetGlobalMousePosition();
            SetProcessInput(true);
            ResizeTarget.AcceptEvent();
        }
    }

    /// <summary>
    /// Enables or disables the resizing mouse cursor.
    /// </summary>
    private void _enableResizingMouseCursor(bool enable)
    {
        ResizeTarget.MouseDefaultCursorShape = enable ? Control.CursorShape.Fdiagsize : Control.CursorShape.Arrow;
    }

    /// <summary>
    /// Handle global input events for resizing.
    /// </summary>
    public override void _Input(InputEvent e)
    {
        if(!_isResizing) return;

        if(e is InputEventMouseMotion)
        {
            var delta = ResizeTarget.GetGlobalMousePosition() - _startMouse;
            var min = ResizeTarget.GetCombinedMinimumSize();
            ResizeTarget.Size = (_startSize + delta).Max(min).Round();
            GetViewport().SetInputAsHandled();
        }
        else if(e is InputEventMouseButton{ ButtonIndex: MouseButton.Left, Pressed: false })
        {
            // Stop resizing
            _isResizing = false;
            SetProcessInput(false);
        }

    }

    /// <summary>
    /// Check that we are in the bottom right corner of the resize target.
    /// </summary>
    /// <param name="position">Current mouse position relative to the resize target.</param>
    /// <returns>True if the mouse is in the bottom right corner of the resize target, false otherwise.</returns>
    private bool InCorner(Vector2 position)
    {
        var rect = ResizeTarget.GetRect();
        return position.X >= rect.Size.X - GrabSize && position.Y >= rect.Size.Y - GrabSize;
    }
}

