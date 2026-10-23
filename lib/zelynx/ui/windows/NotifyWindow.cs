using Godot;

namespace Zelynx.UI.Windows;

/// <summary>
/// A base window class that represents a generic window within the UI.
/// </summary>
public partial class NotifyWindow : Window
{

    public override void _EnterTree()
    {
        base._EnterTree();

        State = new WindowState
        {
            Type = WindowType.Base, 
        };
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }
}
