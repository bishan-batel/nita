using Godot;

namespace Nita.game.mechanic.hittable
{
  public interface IHittable
  {
    [Export] public float KnockbackStrength { set; get; }

    [Export] public float BounceStrength { set; get; }

    void _onHit(HitInformation hitInfo);
  }
}