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

        // TODO simplify damage system away from combat / clean up obsolete code
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
            var overlapping = damageArea.GetOverlappingAreas().Cast<object>()
                // .Where(IsHostile)
                .Cast<IHostile>()
                .Concat(
                    damageArea
                        .GetOverlappingBodies()
                        .Cast<object>()
                        // .Where(IsHostile)
                        .Cast<IHostile>()
                ).ToList();

            if (overlapping.Count is 0) return;

            // // Locate nearest hostile in damage area
            // IHostile nearestHostile = null;
            // float nearestDist = Mathf.Inf;
            // foreach (IHostile hostile in overlapping)
            // {
            //     if (hostile.Disabled) continue;
            //
            //     float dist = ((Node2D) hostile).GlobalPosition.DistanceTo(GlobalPosition);
            //     if (dist >= nearestDist) continue;
            //     nearestDist = dist;
            //     nearestHostile = hostile;
            // }

            // If no hostile areas, exit
            // if (nearestHostile == null) return;

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
            var attackAnim = GetNode<AnimationPlayer>("AttackArea/AnimationPlayer");


            if (attackAnim.IsPlaying())
            {
                Array overlappingAreas = area2D.GetOverlappingAreas();
                if (overlappingAreas.Count == 0) return;

                _processHit(overlappingAreas[0] as IHittable);
                return;
            }

            area2D.Rotation = GetAttackRotation();

            _inKnockback = false;
            if (!Input.IsActionJustPressed("attack")) return;
            attackAnim.Play("attack");
        }

        float GetAttackRotation()
        {
            // Retrieves axis for analog stick
            float
                xPos = Input.GetActionStrength("attack_right"),
                xNeg = Input.GetActionStrength("attack_left"),
                yPos = Input.GetActionStrength("attack_down"),
                yNeg = Input.GetActionStrength("attack_up");

            // Snaps rotation to multiple of AttackRotSnap
            float SnapRot(float rot) => Mathf.Stepify(rot, AttackRotSnap);

            return Input.GetConnectedJoypads().Count == 0
                ? SnapRot(GlobalPosition.AngleToPoint(GetGlobalMousePosition()) + Mathf.Pi - Rotation)
                : SnapRot(new Vector2(xPos - xNeg, yPos - yNeg).Angle() - Rotation);
        }

        void _processHit(IHittable hittable)
        {
            if (_inKnockback || hittable is null) return;

            var area2D = GetNode<Area2D>("AttackArea");

            // Angle away from where attack was clicked
            float angle = area2D.Rotation + Mathf.Pi;
            Vector2 dir = Vector2.Right.Rotated(angle);

            var bounce = 0f;

            // calculates bounce strength if object hitting has a bounce strength factor
            if (hittable.BounceStrength != 0f)
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