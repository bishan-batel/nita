using System.Runtime.Serialization;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using GodotOnReady.Attributes;
using Parry2.game.mechanic;

namespace Parry2.game.world.actors.player
{
    public partial class PlayerShroom : KinematicBody2D, IKillable
    {
        Vector2 _velocity;
        [Export] public float TurnAngle = 10f, TurnSpeed = .5f;

        [Node("Sprite")] readonly Sprite _sprite = null;
        [Node("AttackArea/AnimationPlayer")] readonly AnimationPlayer _attackPlayer = null;
        [Node("AnimationTree")] readonly AnimationTree _animationTree = null;

        [OnReadyGet("AnimationTree", Property = "parameters/playback")]
        AnimationNodeStateMachinePlayback _animPlayback;


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
            _animationTree.Active = true;
            _addDebugCommands();
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
            _attackProcess();
            _sprite.GlobalScale = Vector2.One;
        }
    }
}