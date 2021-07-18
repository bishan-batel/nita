using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.game.room;
using Parry2.managers.save;
using PlayerShroom = Parry2.game.world.actors.player.PlayerShroom;

namespace Parry2.game.world.objects.checkpoint
{
    public class Checkpoint : Node2D, IPersistant
    {
        public const string CheckpointGroup = "Checkpoint";

        static NodePath _claimed;


        // TODO give checkpoint functionality to the room instead of a static class
        [Obsolete("", true)]
        public static NodePath Claimed
        {
            set
            {
                if (value?.ToString() == _claimed?.ToString()) return;
                if (value is null) return;

                // unclaim animation for previous checkpoint if exists
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

        // public ISerializable Save() => new CheckpointSave(ClaimedPath != GetPath());
        public ISerializable Save()
        {
            return null;
        }

        public void LoadFrom(ISerializable obj)
        {
            if (!(obj is CheckpointSave save)) return;

            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play(save.IsClaimed ? "unclaim" : "claim");
        }

        [Obsolete("", true)]
        public static void ClearCheckpoint()
        {
            Claimed = null;
        }

        public void Claim()
        {
            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play("claim");
        }

        public void UnClaim()
        {
            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play("unclaim");
        }

        public void Entered(Node body)
        {
            Room.CheckpointManager.Claim(this);
        }

        [Serializable]
        internal struct CheckpointSave : ISerializable
        {
            public bool IsClaimed;

            public CheckpointSave(bool claimed)
            {
                IsClaimed = claimed;
            }

            public CheckpointSave(SerializationInfo info, StreamingContext context)
            {
                IsClaimed = info.GetBoolean(nameof(IsClaimed));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(IsClaimed), IsClaimed);
            }
        }
    }
}