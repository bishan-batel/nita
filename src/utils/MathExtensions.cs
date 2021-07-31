using Godot;

namespace Nita.utils
{
  public static class MathExtensions
  {
    public static float AngleTo(this Node2D node1, Node2D node2) => node1.GetAngleTo(node2.GlobalPosition);

    /// <summary>
    ///   Used to create a normalized vector in the direction of the specified angle
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector2 AngleToVector2(this float angle) => Vector2.Right.Rotated(angle);
  }
}