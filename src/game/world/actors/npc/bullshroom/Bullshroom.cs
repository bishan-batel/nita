using Godot;
using Nita.game.world.actors.player;
using Nita.utils;

namespace Nita.game.world.actors.npc.bullshroom
{
  public class Bullshroom : CharacterController
  {
    public override void _PhysicsProcess(float delta)
    {
      base._PhysicsProcess(delta);
      AnimationNodeStateMachinePlayback playback = this.GetPlaybackFrom();
      playback.Travel("run");
    }

    protected override Vector2 GetInputDir()
    {
      NodePath playerPath = GameplayScene.CurrentRoom?.PlayerPath;
      if (playerPath == null) return Vector2.Zero;
      var player = GameplayScene.CurrentRoom.GetNode<PlayerShroom>(playerPath);

      bool toRight = player.GlobalPosition.x > GlobalPosition.x;
      var dir = new Vector2(toRight ? 1 : -1, 0f);
      GetNode<Sprite>("Sprite").FlipH = !toRight;
      return dir;
    }
  }
}