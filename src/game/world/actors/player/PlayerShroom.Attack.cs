using System.Linq;
using Godot;
using Godot.Collections;
using Parry2.game.mechanic.hittable;
using Parry2.game.mechanic.hostile;

namespace Parry2.game.world.actors.player
{
  public partial class PlayerShroom
  {
    bool _inKnockback, _invulnerable;
    float _timeInvulnerable;

    // Snaps to every 5 degrees
    [Export] public float AttackRotSnap { get; set; } = Mathf.Deg2Rad(45 * (1 / 4.0f));
    [Export] public int InvulnerabilityFrames { get; set; }
    [Export] public int Health { get; set; }

    void _damageProcess()
    {
      if (_invulnerable || NoClip)
      {
        _timeInvulnerable += GetProcessDeltaTime();

        if (!(_timeInvulnerable > InvulnerabilityFrames * Engine.TargetFps)) return;
        _timeInvulnerable = 0f;
        _invulnerable = false;

        return;
      }

      var damageArea = GetNode<Area2D>("DamageArea");

      // Get all overlapping areas and bodies


      // Get all overlapping bodies and areas of IHostile
      var overlapping = damageArea
          .GetOverlappingAreas()
          .Cast<object>()
          // .Where(IsHostile)
          .Concat(
            damageArea
                .GetOverlappingBodies()
                .Cast<object>()
            // .Where(IsHostile)
          )
          .Cast<IHostile>()
          .ToList();

      if (overlapping.Count is 0) return;

      IHostile hostile = overlapping.First();
      if (_invulnerable) return;

      Input.StartJoyVibration(0, 1f, 1f, .5f);
      if (hostile.TeleportToSafeSpot)
        _rollbackToSafeSpot();

      // Enable invulnerability frames
      _invulnerable = true;
      Health -= hostile.Damage;
    }

    // TODO Death animation
    void _rollbackToSafeSpot()
    {
      GameplayScene
          .CurrentRoom
          .OnDeath();
    }

    void _attackProcess()
    {
      _damageProcess();
      var area2D = GetNode<Area2D>("AttackArea");


      if (_attackPlayer.IsPlaying())
      {
        Array overlappingAreas = area2D.GetOverlappingAreas();
        if (overlappingAreas.Count == 0) return;

        _processHit(overlappingAreas[0] as IHittable);
        return;
      }

      area2D.Rotation = GetAttackRotation();

      _inKnockback = false;
      if (!_controller.AttackJustPressed) return;
      _attackPlayer.Play("attack");
    }

    float GetAttackRotation() => Mathf.Stepify(_controller.AttackRotation, AttackRotSnap);

    void _processHit(IHittable hittable)
    {
      if (_inKnockback || hittable is null) return;

      var area2D = GetNode<Area2D>("AttackArea");

      // Angle away from where attack was clicked
      float angle = area2D.Rotation + Mathf.Pi;
      Vector2 dir = Vector2.Right.Rotated(angle);

      var bounce = 0f;

      // calculates bounce strength if object hitting has a bounce strength factor
      if (hittable.BounceStrength is not 0f)
      {
        float bounceMag = hittable.BounceStrength * _velocity.Length();

        // calculates how close attack direction is to players velocity direction
        float clampedBounceDot =
            Mathf.Clamp(_velocity.Normalized().Dot(dir), -1f, 1f);

        // the closer the attack direction is to velocity direction the higher the bounce effect takes place
        bounce = bounceMag * 1 - (clampedBounceDot + 1f) * .5f;

        if (bounce < 0)
          bounce = 0f;
      }


      Velocity = dir * (hittable.KnockbackStrength + bounce);


      _inKnockback = true;
      IsJumpHold = true;
      hittable._onHit(new HitInformation(this, dir));

      Input.StartJoyVibration(0, .8f, .8f, .2f);
    }
  }
}