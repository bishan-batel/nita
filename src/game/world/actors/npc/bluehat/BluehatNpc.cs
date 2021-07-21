using Godot;
using Parry2.game.mechanic.hittable;
using Parry2.utils;
using PlayerShroom = Parry2.game.world.actors.player.PlayerShroom;

namespace Parry2.game.actors.npc.bluehat
{
    public class BluehatNpc : Node2D
    {
        float _timeInBurrow;
        [Export] public float BurrowTime;

        public override void _Process(float delta)
        {
            var player = GameplayScene
                .CurrentRoom?
                .GetNodeOrNull<PlayerShroom>(GameplayScene.CurrentRoom?.Player);

            if (player is not null && player.IsInsideTree())
            {
                var facing = new Vector2(Mathf.Sin(-Rotation), Mathf.Cos(-Rotation));
                float angle = (player.GlobalPosition - GlobalPosition).AngleTo(facing) + Mathf.Pi;
                GetNode<Sprite>("Sprite").FlipH = angle < Mathf.Pi;
            }

            AnimationNodeStateMachinePlayback playback = this.GetPlaybackFrom();

            if (playback.GetCurrentNode() == "get_up")
                _timeInBurrow = 0f;
            else
                _timeInBurrow += delta;

            if (_timeInBurrow >= BurrowTime)
                playback.Start("get_up");
        }

        public void _on_HittableArea_OnHit(HitInformation info)
        {
            if (info.Attacker is PlayerShroom)
                this
                    .GetPlaybackFrom()
                    .Travel("damage");
        }
    }
}