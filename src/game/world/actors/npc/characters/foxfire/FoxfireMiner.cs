using Godot;

namespace Nita.game.world.actors.npc.characters.foxfire
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