using System.Linq;
using Godot;
using Godot.Collections;

namespace Parry2.game.world.objects.card
{
    [Tool]
    public class KeyCard : Node2D
    {
        [Export]
        public CardColor Color
        {
            set
            {
                _color = value;
                GetNode<Sprite>("Sprite").Frame = (int) value;

                Connections
                    .ToList()
                    .ForEach(path => GetNode<ICardUser>(path).Color = value);
            }
            get => _color;
        }

        [Export] public Array<NodePath> Connections { set; get; } = new();
        CardColor _color;

        public override void _Ready()
        {
            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play("idle");
        }

        public void Entered(object param)
        {
            Connections
                .ToList()
                .ForEach(path => GetNode<ICardUser>(path).OnCardCollected());
            QueueFree();
        }
    }

    public enum CardColor
    {
        White,
        Red,
        Purple,
        Green,
        Yellow
    }
}