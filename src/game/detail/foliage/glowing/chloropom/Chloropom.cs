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
                _energy = value;
                if (IsInsideTree()) GetNode<Light2D>(nameof(Light2D)).Energy = value;
            }
            get => _energy;
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

        float _energy = .87f;


        public override void _Ready()
        {
            Frame = Frame;
            Energy = Energy;
#if DEBUG
            DisplayInEditor = !Engine.EditorHint;
#else
            DisplayInEditor = true;
#endif
            if (Energy == 0)
                Energy = .87f;
        }
    }
}