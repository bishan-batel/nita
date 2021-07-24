using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;

namespace Parry2.game.world.objects.sporevine
{
  [Tool]
  public class Sporevine : Line2D
  {
    [Node("HostileArea/CollisionShape2D")] CollisionShape2D _collisionShape;

    public override void _Ready()
    {
      this.Wire();
      UpdatePoints();
    }

    public void UpdatePoints()
    {
      if (Points is null || Points.Length == 0 || Points.Length > 2)
        Points = new[] {new Vector2(), new Vector2()};

      _collisionShape.Shape = new SegmentShape2D
      {
          A = Points[0],
          B = Points[1]
      };
    }
  }
}