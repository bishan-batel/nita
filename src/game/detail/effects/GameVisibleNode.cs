using Godot;

namespace Nita.game.detail.effects
{
  [Tool]
  public class GameVisibleNode : Node2D
  {
#if DEBUG
    public override void _Ready()
    {
      Visible = !Engine.EditorHint;
    }
#endif
  }
}