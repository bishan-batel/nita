using Godot;
using Parry2.game.mechanic;

namespace Parry2.game.actors.player
{
    public partial class PlayerShroom : KinematicBody2D, IKillable
    {
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

        public override void _Ready()
        {
            _velocity = new Vector2();
            GetNode<AnimationTree>("AnimationTree").Active = true;
        }

        public void _turnAnim()
        {
            float target = 0;

            if (!IsOnFloor())
                target = _velocity.x / MaxSpeed * -TurnAngle;

            var sprite = GetNode<Sprite>("Sprite");
            sprite.RotationDegrees += (target - sprite.RotationDegrees) * TurnSpeed;
        }


        public override void _Process(float delta)
        {
            _attackProcess();

            var sprite = GetNode<Sprite>("Sprite");
            sprite.GlobalScale = Vector2.One;
        }
    }
}