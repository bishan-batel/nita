using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.managers.save;
using Parry2.utils;

namespace Parry2.game.actors.npc
{
    public abstract class CharacterController : KinematicBody2D
    {
        [Export] public float MaxSpeed, Accel, DeAccel, Gravity = 200f;
        protected Vector2 Velocity;
        bool _onFloor;


        protected void Decelerate(float delta)
        {
            int prevSign = Mathf.Sign(Velocity.x);
            Velocity.x += -prevSign * DeAccel * delta;

            if (Mathf.Sign(Velocity.x) != prevSign)
                Velocity.x = 0;
        }

        public override void _PhysicsProcess(float delta)
        {
            Velocity = MoveAndSlide(Velocity, Vector2.Up);
            _onFloor = IsOnFloor();
            Vector2 dir = GetInputDir();

            if (Mathf.Sign(dir.x) != Mathf.Sign(Velocity.x) || dir.x == 0)
                Decelerate(delta);

            Velocity.x += Accel * dir.x * delta;
            Velocity.x = Mathf.Clamp(Velocity.x, -MaxSpeed, MaxSpeed);
            Velocity.y += Gravity * delta;
        }

        protected virtual Vector2 GetInputDir()
        {
            return Vector2.Zero;
        }
    }
}