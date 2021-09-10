using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;

namespace Nita.game.world.objects.sporevine
{
  [Tool]
  public class Sporevine : Line2D
  {
    [Node("HostileArea/CollisionShape2D")] readonly CollisionShape2D _collisionShape = null;

    public override void _Ready()
    {
      this.Wire();
      UpdatePoints();
    }

    public void UpdatePoints()
    {
      if (Points is null || Points.Length is 0 or > 2)
        Points = new Vector2[2];

      _collisionShape.Shape = new SegmentShape2D
      {
        A = Points[0],
        B = Points[1]
      };
    }
  }
}