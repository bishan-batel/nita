using System;
using Godot;
using Parry2.managers.save;

namespace Parry2.game.world.objects.checkpoint
{
    public class Checkpoint : Node2D
    {
        public const string CheckpointGroup = "Checkpoint";

        // TODO give checkpoint functionality to the room instead of a static class
        public static NodePath Claimed
        {
            set
            {
                if (value?.ToString() == _claimed?.ToString()) return;

                Global
                    .Singleton
                    .GetNodeOrNull<Checkpoint>(_claimed ?? "")
                    ?.UnClaim();

                Global
                    .Singleton
                    .GetNode<Checkpoint>(_claimed = value)
                    .Claim();

                SaveManager.Save();
                GD.Print("Hit checkpoint");
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
    }
}