using Godot;
using Parry2.game.mechanic.hittable;

namespace Parry2.game.world.objects.ilkspring
{
    public class Ilkspring : Node2D
    {
        Timer _timer;
        [Export] public float DownTime = 3;

        public void Timeout()
        {
            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play("unsquish");

            GetNode<HittableArea>(nameof(HittableArea))
                .GetNode<CollisionShape2D>(nameof(CollisionShape2D))
                .Disabled = false;
            _timer?.QueueFree();
        }

        // ReSharper disable once UnusedParameter.Global
        public void _on_HittableArea_OnHit(HitInformation info)
        {
            GetNode<AnimationPlayer>("AnimationPlayer")
                .Play("squish");

            GetNode<HittableArea>(nameof(HittableArea))
                .GetNode<CollisionShape2D>(nameof(CollisionShape2D))
                .Disabled = true;

            _timer = new Timer();
            AddChild(_timer);
            _timer.Start(DownTime);
            _timer.Connect("timeout", this, nameof(Timeout));
        }
    }
}