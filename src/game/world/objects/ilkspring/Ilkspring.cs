using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Parry2.game.mechanic.hittable;

namespace Parry2.game.world.objects.ilkspring
{
  public class Ilkspring : Node2D
  {
    [Node("HittableArea/CollisionShape2D")]
    readonly CollisionShape2D _hittableCollision = null;

    [Node("AnimationPlayer")] readonly AnimationPlayer _player = null;
    Timer _timer;

    [Export] public float DownTime = 3;

    public override void _Ready() => this.Wire();

    public void Timeout()
    {
      _player.Play("unsquish");
      _hittableCollision.Disabled = false;
      _timer?.QueueFree();
    }

    // ReSharper disable once UnusedParameter.Global
    public void _on_HittableArea_OnHit(HitInformation info)
    {
      _player.Play("squish");
      _hittableCollision.Disabled = true;

      _timer = new Timer();
      AddChild(_timer);
      _timer.Start(DownTime);
      _timer.Connect("timeout", this, nameof(Timeout));
    }
  }
}