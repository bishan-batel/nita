using Godot;
using Parry2.utils;

namespace Parry2.game.world.actors.player
{
    public partial class PlayerShroom
    {
        [Export] public float
            Accel,
            DeAccel,
            FastDeAccel,
            MaxSpeed;

        [Export] public float
            Gravity,
            JumpRiseMultiplier,
            JumpHoldFallMultiplier,
            JumpFallMultiplier,
            JumpStrength,
            MaxFall;

        public override void _PhysicsProcess(float delta)
        {
            _velocity = MoveAndSlide(_velocity, Vector2.Up);
            _turnAnim();

            if (NoClip)
            {
                _noClipMovement();
                return;
            }

            _processMovement(delta);
            _processJump(delta);
        }

        void _noClipMovement()
        {
            Velocity = new Vector2(
                Input.GetActionStrength("right") - Input.GetActionStrength("left"),
                Input.GetActionStrength("down") - Input.GetActionStrength("up")
            ) * NoClipSpeed;
        }

        void _processJump(float delta)
        {
            // Jump multiplier stuff to make it seem more dynamic
            float multiplier;
            if (_velocity.y > 0)
                multiplier = IsJumpHold ? JumpHoldFallMultiplier : JumpFallMultiplier;
            else
                multiplier = IsJumpHold ? JumpRiseMultiplier : JumpFallMultiplier;

            _velocity.y += Gravity * multiplier * delta;
            _velocity.y = Mathf.Min(_velocity.y, MaxFall);

            // Jump impulse and hold detection
            if (IsOnFloor())
            {
                if (!Input.IsActionJustPressed("jump") || !ControlActive) return;
                _velocity.y = -JumpStrength;
                IsJumpHold = true;
                _animPlayback.Travel("jump");
            }
            else if (IsJumpHold && ControlActive)
            {
                IsJumpHold = Input.IsActionPressed("jump");
            }
        }

        void _processMovement(float delta)
        {
            float input = Input.GetActionStrength("right") - Input.GetActionStrength("left");

            if (input == 0 && ControlActive)
            {
                _decelerate(delta);
                if (IsOnFloor()) _animPlayback.Travel("idle");
            }
            else
            {
                _sprite.FlipH =
                    (ControlActive ? Velocity.x : input) < 0;
                if (IsOnFloor()) _animPlayback.Travel("run");
            }

            if (Mathf.Sign(_velocity.x) != Mathf.Sign(input))
                _decelerate(delta);

            float force = input * delta * Accel;

            if (Mathf.Abs(_velocity.x) < MaxSpeed)
                _velocity.x += force;
            else
                _decelerate(delta, true);
        }

        void _decelerate(float delta, bool altAccel = false)
        {
            float accel = altAccel ? FastDeAccel : DeAccel;
            int prevSign = Mathf.Sign(_velocity.x);
            _velocity.x += -prevSign * accel * delta;

            // Prevents overshooting / wobble effect
            if (prevSign != Mathf.Sign(_velocity.x))
                _velocity.x = 0f;
        }

        public void ApplyForce(Vector2 force) => _velocity += force;
    }
}