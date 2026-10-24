using System.Collections.Generic;

using Godot;

namespace Zelynx.UI.Windows;

/// <summary>
/// A component that allows the parent window to be dragged using a specified handle.
/// </summary>
[GlobalClass, Tool, Icon("res://lib/zelynx/icons/draggable_16x16.svg")]
public partial class Draggable: Node
{
    #region States

    [Export] public bool IsDraggable { get; set; }   = true;  
    private bool _isDragging;
    [Export] public bool ConstrainToViewport { get; set; } = false;

    #endregion

    #region Object References

    /// <summary>
    /// The control that acts as the handle for dragging the parent window.
    /// </summary>
    [Export] public Control DragHandle { get; set; }


    /// <summary>
    /// The control that will be moved when the drag handle is dragged.
    /// </summary>
    [Export] public Control DragTarget { get; set; } 

    #endregion

    #region Coordinates

    private Vector2 _dragOffset;
    private Vector2 _viewportMaxBounds;

    #endregion


    /// <summary>
    /// Initializes the draggable component and sets up the drag handle input events.
    /// </summary>
    public override void _Ready()
    {
        // Skip runtime setup if in editor, which provides its own handling as a tool.
        if (Engine.IsEditorHint())
        {
            return;
        }

        SetProcessInput(false); // Disable _Input until we know we are dragging

        if(DragHandle == null || DragTarget == null)
        {
            GD.PushError("Draggable component requires both a DragHandle and a DragTarget.");
            return;
        }

        DragHandle.MouseFilter = Control.MouseFilterEnum.Pass;
        DragHandle.GuiInput += OnDragHandleGuiInput;
    }

    public override void _ExitTree() => _isDragging = false;

    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusOut)
        {
            _isDragging = false;
        }
    }

    /// <summary>
    /// Attached directly to the drag handle. We can track global mouse movement in _Input, so we only need to track the initial press here.
    /// </summary>
    private void OnDragHandleGuiInput(InputEvent e)
    {
        if(e is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            // If the handle is being dragged, start the drag state.
            if(mouseEvent.Pressed && IsDraggable)
            {
                _dragOffset = DragTarget.GetGlobalMousePosition() - DragTarget.GlobalPosition;
                _isDragging = true;
                _UpdateViewportBounds();
                SetProcessInput(true);
            }
        }
    }

    /// <summary>
    /// Handles input events only while dragging.
    /// </summary>
    public override void _Input(InputEvent e)
    {
        switch (e)
        {
            // Drag logic, sets position while checking bounds and avoiding sub-pixel positioning
            case InputEventMouseMotion mouseMotionEvent when _isDragging:
                if (!ConstrainToViewport)
                {
                    DragTarget.GlobalPosition = DragTarget.GetGlobalMousePosition() - _dragOffset;
                } 
                else
                {
                    DragTarget.GlobalPosition = (DragTarget.GetGlobalMousePosition() - _dragOffset).Clamp(Vector2.Zero, _viewportMaxBounds).Round();
                }
                GetViewport().SetInputAsHandled(); 
                break;

            case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }:
                if(_isDragging)
                {
                    _isDragging = false;
                    SetProcessInput(false);
                }
                break;
        }
    }

    /// <summary>
    /// Internal method for updating the viewport bounds when needed. Generally on drag start/stop.
    /// </summary>
    /// <returns></returns>
    private void _UpdateViewportBounds()
    {
        _viewportMaxBounds = (DragTarget.GetViewportRect().Size - DragTarget.Size).Max(Vector2.Zero);
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (DragTarget == null)
        {
            warnings.Add("DragTarget is not assigned.");
        }

        if (DragHandle == null)
        {
            warnings.Add("DragHandle is not assigned.");
        }

        return warnings.ToArray();
    }
}