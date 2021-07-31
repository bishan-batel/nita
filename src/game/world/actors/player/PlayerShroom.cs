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
    [Node("AnimationTree")] readonly AnimationTree _animationTree = null;

    readonly Func<InputController, InputController> _applyController = InputManager.Apply;
    [Node("AttackArea/AnimationPlayer")] readonly AnimationPlayer _attackPlayer = null;

    [Node("Sprite")] readonly Sprite _sprite = null;

    [OnReadyGet("AnimationTree", Property = "parameters/playback")]
    AnimationNodeStateMachinePlayback _animPlayback;

    InputController _controller;
    Vector2 _velocity;
    [Export] public float TurnAngle = 10f, TurnSpeed = .5f;


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

      _sprite.RotationDegrees += (target - _sprite.RotationDegrees) * TurnSpeed;
    }

    public override void _Process(float delta)
    {
      _controller = _applyController.Invoke(_controller);

      _attackProcess();
      _sprite.GlobalPosition = GlobalPosition.Round();
      _sprite.Scale = Vector2.One;
    }
  }
}