using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.game.actors.player;
using Parry2.game.mechanic.hittable;
using Parry2.managers.save;
using Parry2.utils;

namespace Parry2.game.actors.npc.bluehat
{
    public class BluehatNpc : Node2D, IPersistant
    {
        float _timeInBurrow;
        [Export] public float BurrowTime;

        public ISerializable Save()
        {
            return new BluehatNpcSave
            {
                Playback = this.GetPlayback().GetCurrentNode(),
                TimeInBurrow = _timeInBurrow
            };
        }

        public void LoadFrom(ISerializable obj)
        {
            if (!(obj is BluehatNpcSave save)) return;
            _timeInBurrow = save.TimeInBurrow;

            if (save.Playback == "") return;
            this
                .GetPlayback()
                .Start(save.Playback);
        }

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

            AnimationNodeStateMachinePlayback playback = this.GetPlayback();

            if (playback.GetCurrentNode() == "get_up")
                _timeInBurrow = 0f;
            else
                _timeInBurrow += delta;

            if (_timeInBurrow >= BurrowTime)
                playback.Start("get_up");
        }

        public void _on_HittableArea_OnHit(HitInformation info)
        {
            if (!(info.Attacker is PlayerShroom)) return;

            AnimationNodeStateMachinePlayback playback =
                this.GetPlayback();

            playback.Travel("damage");
        }

        [Serializable]
        internal class BluehatNpcSave : ISerializable
        {
            public string Playback;
            public float TimeInBurrow;

            public BluehatNpcSave(SerializationInfo info = null, StreamingContext context = default)
            {
                if (info is null) return;
                Playback = info.GetString(nameof(Playback));
                TimeInBurrow = info.GetSingle(nameof(TimeInBurrow));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(Playback), Playback);
                info.AddValue(nameof(TimeInBurrow), TimeInBurrow);
            }
        }
    }
}