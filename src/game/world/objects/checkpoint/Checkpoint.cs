using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.game.actors.player;
using Parry2.managers.save;

namespace Parry2.game.world.objects.checkpoint
{
    public class Checkpoint : Node2D, IPersistant
    {
        public const string CheckpointGroup = "Checkpoint";

        // TODO give checkpoint functionality to the room instead of a static class
        public static NodePath Claimed
        {
            set
            {
                if (value?.ToString() == _claimed?.ToString()) return;

                // Unclaim animation for previous checkpoint if exists
                Global
                    .Singleton
                    .GetNodeOrNull<Checkpoint>(_claimed ?? "")
                    ?.UnClaim();

                // Claim animation for checkpoint
                var checkpoint = Global
                    .Singleton
                    .GetNode<Checkpoint>(_claimed = value);
                checkpoint.Claim();
                // Swap player positions so they save in the center position of checkpoint

                var player = Global
                    .Singleton
                    .GetNodeOrNull<PlayerShroom>(GameplayScene.CurrentRoom?.Player);
                Vector2? oldPos = null;

                if (player is not null)
                {
                    oldPos = player.GlobalPosition;
                    player.GlobalPosition = checkpoint.GlobalPosition;
                }

                SaveManager.SaveDeferred();

                // Swamps player position back
                if (player is not null)
                    player.GlobalPosition = (Vector2) oldPos;

                GD.Print($"Checkpoint hit @ {value}");
            }

            get => _claimed;
        }

        static NodePath _claimed;

        public void Claim() =>
            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play("claim");

        public void UnClaim() =>
            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play("unclaim");

        public void Entered(Node body) => Claimed = GetPath();

        public ISerializable Save() => new CheckpointSave(Claimed != GetPath());

        public void LoadFrom(ISerializable obj)
        {
            if (!(obj is CheckpointSave save)) return;

            this.GetNode<AnimationPlayer>("AnimationPlayer")
                .Play(save.IsClaimed ? "unclaim" : "claim");
        }

        [Serializable]
        internal struct CheckpointSave : ISerializable
        {
            public bool IsClaimed;
            public CheckpointSave(bool claimed) => IsClaimed = claimed;

            public CheckpointSave(SerializationInfo info, StreamingContext context) =>
                IsClaimed = info.GetBoolean(nameof(IsClaimed));

            public void GetObjectData(SerializationInfo info, StreamingContext context) =>
                info.AddValue(nameof(IsClaimed), IsClaimed);
        }
    }
}