using System;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using GodotOnReady.Attributes;
using Nita.game.mechanic;
using Nita.managers;

namespace Nita.game.world.actors.player
{
  // #girlboss
  public partial class PlayerShroom : KinematicBody2D, IKillable
  {
    public enum State
    {
      IdleWalk,
      Sleeping,
      Sitting,
      InAir,
      InCutscene
    }

    [Export] public float TurnAngle = 10f, TurnSpeed = .5f;
    [Node("AnimationTree")] readonly AnimationTree _animationTree = null;

    [Node("AttackArea/AnimationPlayer")] readonly AnimationPlayer _attackPlayer = null;

    [Node("Sprite")] readonly Sprite _sprite = null;

    public State CurrentState;

    [OnReadyGet("AnimationTree", Property = "parameters/playback")]
    AnimationNodeStateMachinePlayback _animPlayback;

    InputController _controller;
    readonly Func<InputController, InputController> _applyController = InputManager.Apply;

    Vector2 _velocity;
    float _speedRot;

    public Vector2 Velocity
    {
      set => _velocity = value;
      get => _velocity;
    }

    public bool IsJumpHold { set; get; }
    public bool ControlActive { get; set; } = true;

    public void Kill()
    {
    }

    [OnReady]
    public void OnReady()
    {
      this.Wire();
      _velocity = Vector2.Zero;
      _controller = new InputController(this);
      _addDebugCommands();

      _animationTree.Active = true;
      _animPlayback.Start("sleeping");
    }


    void _turnAnim()
    {
      float target = 0;

      if (!IsOnFloor())
        target = _velocity.x / MaxSpeed * -TurnAngle;

      _speedRot += (target - _speedRot) * TurnSpeed;
      _sprite.RotationDegrees = Mathf.Round(_speedRot);
    }

    public override void _Process(float delta)
    {
      _controller = _applyController.Invoke(_controller);

      _attackProcess();
      _sprite.GlobalPosition = GlobalPosition.Round();
      _sprite.Scale = Vector2.One;
    }

    // Sound Signals
    [Signal] public delegate void OnJump();

    [Signal] public delegate void OnLand();

    [Signal] public delegate void OnHit();
  }
}