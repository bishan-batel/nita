using Godot;

namespace Parry2.game.mechanic.hittable
{
  public class HittableArea : Area2D, IHittable
  {
    [Signal]
    public delegate void OnHit(HitInformation hitInfo);

    [Export] public float KnockbackStrength { get; set; }
    [Export] public float BounceStrength { get; set; }

    void IHittable._onHit(HitInformation hitInfo)
    {
      EmitSignal(nameof(OnHit), hitInfo);
    }
  }
}