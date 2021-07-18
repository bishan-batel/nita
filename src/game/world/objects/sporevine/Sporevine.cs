using Godot;

namespace Parry2.game.world.objects.sporevine
{
    [Tool]
    public class Sporevine : Line2D
    {
        public override void _Ready()
        {
            UpdatePoints();
        }

        public void UpdatePoints()
        {
            var collisionShape = GetNode<CollisionShape2D>("HostileArea/CollisionShape2D");

            if (Points is null || Points.Length == 0 || Points.Length > 2)
                Points = new[] {new Vector2(), new Vector2()};

            collisionShape.Shape = new SegmentShape2D
            {
                A = Points[0],
                B = Points[1]
            };
        }
    }
}