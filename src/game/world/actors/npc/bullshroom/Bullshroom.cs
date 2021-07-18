using Godot;
using Parry2.utils;
using PlayerShroom = Parry2.game.world.actors.player.PlayerShroom;

namespace Parry2.game.actors.npc.bullshroom
{
    public class Bullshroom : CharacterController
    {
        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            AnimationNodeStateMachinePlayback playback = this.GetPlayback();
            playback.Travel("run");
        }

        protected override Vector2 GetInputDir()
        {
            NodePath playerPath = GameplayScene.CurrentRoom?.Player;
            if (playerPath == null) return Vector2.Zero;
            var player = GameplayScene.CurrentRoom.GetNode<PlayerShroom>(playerPath);

            bool toRight = player.GlobalPosition.x > GlobalPosition.x;
            var dir = new Vector2(toRight ? 1 : -1, 0f);
            GetNode<Sprite>("Sprite").FlipH = !toRight;
            return dir;
        }
    }
}