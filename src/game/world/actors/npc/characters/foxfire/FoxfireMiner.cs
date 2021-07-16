using Godot;

namespace Parry2.game.actors.npc.characters.foxfire
{
    public class FoxfireMiner : Node2D
    {
        public override void _Ready()
        {
        }

        public override void _Process(float delta)
        {
            GetNode<Sprite>(nameof(Sprite)).RotationDegrees += 5f;
        }
    }
}