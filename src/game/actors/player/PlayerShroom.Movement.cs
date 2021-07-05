using Godot;
using Parry2.utils;

namespace Parry2.game.actors.player
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
            _processMovement(delta);
            _processJump(delta);
        }

        void _processJump(float delta)
        {
            AnimationNodeStateMachinePlayback playback = this.GetPlayback();

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
                if (!Input.IsActionJustPressed("jump")) return;
                _velocity.y = -JumpStrength;
                IsJumpHold = true;
                playback.Travel("jump");
            }
            else if (IsJumpHold)
            {
                IsJumpHold = Input.IsActionPressed("jump");
            }
        }

        void _processMovement(float delta)
        {
            AnimationNodeStateMachinePlayback stateMachine = this.GetPlayback();

            float input = Input.GetActionStrength("right") - Input.GetActionStrength("left");

            if (input == 0)
            {
                _decelerate(delta);
                if (IsOnFloor()) stateMachine.Travel("idle");
            }
            else
            {
                GetNode<Sprite>("Sprite").FlipH = input < 0;
                if (IsOnFloor()) stateMachine.Travel("run");
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

        public void ApplyForce(Vector2 force)
        {
            _velocity += force;
        }
    }
}