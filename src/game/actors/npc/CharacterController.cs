using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.managers.save;
using Parry2.utils;

namespace Parry2.game.actors
{
    public abstract class CharacterController : KinematicBody2D, IPersistant
    {
        [Export] public float MaxSpeed, Accel, DeAccel, Gravity = 200f;
        protected Vector2 Velocity;

        public ISerializable Save()
        {
            return new CharacterControllerSave
            {
                Position = GlobalPosition,
                Velocity = Velocity
            };
        }

        public void LoadFrom(ISerializable obj)
        {
            if (!(obj is CharacterControllerSave save)) return;
            Velocity = save.Velocity;
            GlobalPosition = save.Position;
        }

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

        [Serializable]
        public struct CharacterControllerSave : ISerializable
        {
            internal Vector2Serial Velocity, Position;

            public CharacterControllerSave(SerializationInfo info, StreamingContext context)
            {
                Velocity = (Vector2Serial) info.GetValue(nameof(Velocity), typeof(Vector2Serial));
                Position = (Vector2Serial) info.GetValue(nameof(Position), typeof(Vector2Serial));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(Velocity), Velocity);
                info.AddValue(nameof(Position), Position);
            }
        }
    }
}