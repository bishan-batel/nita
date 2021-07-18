using System.Collections;
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
        [Export] public float AttackRotSnap = Mathf.Deg2Rad(45);
        [Export] public int InvulnerabilityFrames;
        [Export] public int Health { get; set; }

        // TODO simplify damage system away from combat / clean up obsolete code
        void _damageProcess()
        {
            if (_invulnerable)
            {
                _timeInvulnerable += GetProcessDeltaTime();
                if (_timeInvulnerable > InvulnerabilityFrames * Engine.TargetFps)
                {
                    _timeInvulnerable = 0f;
                    _invulnerable = false;
                }
            }

            var damageArea = GetNode<Area2D>("DamageArea");

            // Get all overlapping areas and bodies
            Array overlapping = damageArea.GetOverlappingAreas();
            foreach (object body in damageArea.GetOverlappingBodies())
                if (body is IHostile)
                    overlapping.Add(body);

            // Locate nearest hostile in damage area
            IHostile nearestHostile = null;
            float nearestDist = Mathf.Inf;
            foreach (object overlap in overlapping)
            {
                var hostile = (IHostile) overlap;
                if (hostile.Disabled) continue;

                var node = (Node2D) overlap;
                float dist = node.GlobalPosition.DistanceTo(GlobalPosition);
                if (dist >= nearestDist) continue;
                nearestDist = dist;
                nearestHostile = hostile;
            }

            // If no hostile areas, exit
            if (nearestHostile == null) return;

            Input.StartJoyVibration(0, 1f, 1f, .5f);
            Input.StartJoyVibration(1, 1f, 1f, .5f);
            if (nearestHostile.TeleportToSafeSpot)
                _rollbackToSafeSpot();

            if (_invulnerable) return;
            // Enable invulnerability frames
            _invulnerable = true;
            Health -= nearestHostile.Damage;
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
                _processHit(area2D.GetOverlappingAreas());
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
            float SnapRot(float rot)
            {
                return Mathf.Stepify(rot, AttackRotSnap);
            }

            return SnapRot(
                Mathf.Abs(xPos + xNeg + yPos + yNeg) == 0
                    // if analog stick is not being used, calculate angle from mouse position
                    ? GlobalPosition.AngleToPoint(GetGlobalMousePosition()) + Mathf.Pi - Rotation

                    // else, use analog stick for input
                    : new Vector2(xPos - xNeg, yPos - yNeg).Angle() - Rotation
            );
        }

        void _processHit(IList bodies)
        {
            if (_inKnockback || bodies.Count == 0) return;

            var hittable = (IHittable) bodies[0];
            var area2D = GetNode<Area2D>("AttackArea");

            // Angle away from where attack was clicked
            float angle = area2D.Rotation + Mathf.Pi;
            var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

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
            Input.StartJoyVibration(1, .8f, .8f, .2f);
        }
    }
}