using Godot;

namespace Parry2.game.mechanic.hittable
{
    // would use a struct but godot has as stroke when using them in signals
    public class HitInformation : Resource

    {
        public readonly float Angle;
        public readonly object Attacker;

        public HitInformation(object attacker, Vector2 direction)
            : this(attacker, direction.Angle())
        {
        }

        public HitInformation(object attacker, float angle)
        {
            Attacker = attacker;
            Angle = angle;
        }

        public Vector2 Direction => Vector2.Right.Rotated(Angle);
    }
}