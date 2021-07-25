using Godot;

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

    bool _onGroundLastFrame;

    public override void _PhysicsProcess(float delta)
    {
      _velocity = MoveAndSlide(_velocity, Vector2.Up);
      if (!_onGroundLastFrame && IsOnFloor())
        Input.StartJoyVibration(0, .4f, .4f, .21f);
      _onGroundLastFrame = IsOnFloor();

      _turnAnim();

      if (NoClip)
      {
        _noClipMovement();
        return;
      }

      _processMovement(delta);
      _processJump(delta);
      _sprite.GlobalPosition = _sprite.GlobalPosition.Round();
    }

    void _noClipMovement()
    {
      Velocity = _controller.WalkInput * NoClipSpeed;
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
        if (!_controller.JumpJustPressed || !ControlActive) return;
        _velocity.y = -JumpStrength;
        IsJumpHold = true;
        _animPlayback.Travel("jump");
      }
      else if (IsJumpHold && ControlActive)
      {
        IsJumpHold = _controller.JumpPressed;
      }
    }

    void _processMovement(float delta)
    {
      float input = _controller.WalkInput.x;

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