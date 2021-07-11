using Godot;
using Parry2.editor;

namespace Parry2.game
{
    [Tool]
    public class LayerHelper : Node
    {
        // Used as a sort of button within the godot editor
        [Export]
        public bool OrganizeLayers
        {
            // ReSharper disable once ValueParameterNotUsed
            set => ObjectOrder.OrganizeLayersInTree(IsInsideTree() ? GetTree() : null);
            get => false;
        }

        [Export] public float OrganizeInterval = 1.0f;

#if DEBUG
        Timer _timer;
#endif
        public override void _Ready()
        {
            OrganizeLayers = true;
#if DEBUG
            if (!Engine.EditorHint) return;
            AddChild(_timer = new Timer {OneShot = true});
            _timer.Connect("timeout", this, nameof(Timeout));
            _timer.Start(OrganizeInterval);
#endif
        }

#if DEBUG
        public void Timeout()
        {
            OrganizeLayers = true;
            _timer.Start(OrganizeInterval);
        }
#endif
    }
}