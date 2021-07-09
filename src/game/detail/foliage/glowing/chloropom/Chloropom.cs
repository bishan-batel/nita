using Godot;

namespace Parry2.game.detail.foliage.glowing.chloropom
{
    [Tool]
    public class Chloropom : Sprite
    {
        [Export]
        public new int Frame
        {
            set => base.Frame = value;
            get => base.Frame;
        }

        [Export]
        public float Energy
        {
            set
            {
                if (IsInsideTree()) GetNode<Light2D>(nameof(Light2D)).Energy = value;
            }
            get => IsInsideTree() ? GetNode<Light2D>(nameof(Light2D)).Energy : 0;
        }

        [Export]
        public bool DisplayInEditor
        {
            set
            {
                if (IsInsideTree()) GetNode<Light2D>(nameof(Light2D)).Visible = value;
            }
            get => IsInsideTree() && GetNode<Light2D>(nameof(Light2D)).Visible;
        }


        public override void _Ready()
        {
            Frame = Frame;
#if DEBUG
            DisplayInEditor = !Engine.EditorHint;
#else
            DisplayInEditor = true;
#endif
        }
    }
}