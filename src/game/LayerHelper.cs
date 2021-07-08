using Godot;
using Parry2.editor;

namespace Parry2.game
{
    [Tool]
    public class LayerHelper : Node
    {
        [Export]
        public bool OrganizeLayers
        {
            set => ObjectOrder.OrganizeLayersInTree(IsInsideTree() ? GetTree() : null);
            get => false;
        }

        public override void _Ready() => OrganizeLayers = OrganizeLayers;
    }
}