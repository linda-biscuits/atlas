using Godot;

namespace Zelynx.UI.Components;

public partial class TitleBarComponent : PanelContainer
{
        private Label _windowTitleLabel;
        private TextureRect _windowTitleIcon;
        private Button _closeButton;
        private Button _maximizeButton;
        private Button _minimizeButton;

        private bool _hasMinimize = false;
        private bool _hasMaximize = false;
        private bool _hasClose = false;

        public override void _Ready()
        {
            _windowTitleLabel = GetNode<Label>("%Title");
            _windowTitleIcon = GetNode<TextureRect>("%TitleIcon");
            _minimizeButton = GetNode<Button>("%Minimize");
            _maximizeButton = GetNode<Button>("%Maximize");
            _closeButton = GetNode<Button>("%Close");

        }

        public void Init(string title, Texture2D icon, bool hasMinimize = false, bool hasMaximize = false, bool hasClose = false)
        {
            // Populate title
            _windowTitleLabel.Text = title;

            // Populate icon if provided, otherwise default
            if(icon != null)
            { 
                _windowTitleIcon.Texture = icon;
            }

            _hasMinimize = hasMinimize;
            _hasMaximize = hasMaximize;
            _hasClose = hasClose;

            _closeButton.Visible = _hasClose;
            _closeButton.Disabled = !_hasClose;
            _maximizeButton.Visible = _hasMaximize;
            _maximizeButton.Disabled = !_hasMaximize;
            _minimizeButton.Visible = _hasMinimize;
            _minimizeButton.Disabled = !_hasMinimize;
        }
}