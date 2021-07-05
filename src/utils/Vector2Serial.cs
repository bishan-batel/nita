using System;
using System.Runtime.Serialization;
using Godot;

namespace Parry2.utils
{
    [Serializable]
    public struct Vector2Serial : ISerializable
    {
        // ReSharper disable twice InconsistentNaming
        public float x, y;

        public Vector2Serial(Vector2 v) : this(v.x, v.y)
        {
        }

        public Vector2Serial(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2Serial(Vector2 v)
        {
            return new Vector2Serial(v);
        }

        public static implicit operator Vector2(Vector2Serial s)
        {
            return new Vector2(s.x, s.y);
        }

        public Vector2Serial(SerializationInfo info, StreamingContext context)
        {
            x = info.GetSingle(nameof(x));
            y = info.GetSingle(nameof(y));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(x), x);
            info.AddValue(nameof(y), y);
        }
    }
}