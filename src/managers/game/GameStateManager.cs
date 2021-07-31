using Godot;

namespace Nita.managers.game
{
  public class GameStateManager : Node
  {
    public static GameStateManager Singleton;
    public static GameState CurrentState { set; get; }

    public override void _Ready()
    {
      Singleton = this;
    }
  }
}