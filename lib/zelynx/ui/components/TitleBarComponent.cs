using Godot;

namespace Zelynx.UI.Components;

public partial class TitleBarComponent : PanelContainer
{
    [Export]public Control ParentWindow { get; set; }
    [Export]public TextureRect Icon;
    [Export]public string Title = "Untitled";
    [Export]public bool HasMinimize = false;
    [Export]public bool HasMaximize = false;
    [Export]public bool HasClose = false;

    private Label _windowTitleLabel;
    private TextureRect _titleIcon;
    private Button _closeButton;
    private Button _maximizeButton;
    private Button _minimizeButton;

    public override void _Ready()
    {
        _windowTitleLabel = GetNode<Label>("%Title");
        _titleIcon = GetNode<TextureRect>("%TitleIcon");
        _minimizeButton = GetNode<Button>("%Minimize");
        _maximizeButton = GetNode<Button>("%Maximize");
        _closeButton = GetNode<Button>("%Close");
    

        if(!string.IsNullOrEmpty(Title))
        {
            _windowTitleLabel.Text = Title;
        }

        // Populate icon if provided, otherwise default
        if(Icon != null)
        { 
            _titleIcon.Texture = Icon.Texture;
        }

        if(ParentWindow == null){ GD.PrintErr("ParentWindow is not assigned."); return; }

        // Configure close
        _closeButton.Visible = HasClose;
        _closeButton.Disabled = !HasClose;
        if (HasClose)
        {
            _closeButton.Pressed += () => ParentWindow.Call("Close");
        }

        // Configure maximize
        _maximizeButton.Visible = HasMaximize;
        _maximizeButton.Disabled = !HasMaximize;
        if (HasMaximize)
        {
            _maximizeButton.Pressed += () => ParentWindow.Call("Maximize");
        }

        // Configure minimize
        _minimizeButton.Visible = HasMinimize;
        _minimizeButton.Disabled = !HasMinimize;
        if (HasMinimize)
        {
            _minimizeButton.Pressed += () => ParentWindow.Call("Minimize");
        }

    }

}