using System.Linq;
using Godot;

namespace Nita.game.detail.rope
{
  [Tool]
  public class Rope : Line2D
  {
    [Export] public NodePath AnchorPath;

    [Export]
    public int NodeCount
    {
      set
      {
        _nodeCount = value;
        GeneratePoints();
      }
      get => _nodeCount;
    }

    [Export] public float AdditionalBinding;
    [Export] public float Gravity = 1f;
    [Export(PropertyHint.Range, "0,1")] public float Tension = .5f;
    [Export] public bool ConstrainEndPoint { get; set; } = true;

    int _nodeCount;
    private Segment[] _segments;

    public override void _Ready()
    {
      GeneratePoints();
    }

    public void GeneratePoints()
    {
      if (!IsInsideTree()) return;
      var endPoint = GetNodeOrNull<Node2D>(AnchorPath ?? GetPath());
      if (endPoint == null) return;

      _segments = new Segment[NodeCount + 1];


      for (var i = 0; i <= NodeCount; i++)
      {
        _segments[i] = new Segment(
          Vector2.Zero.LinearInterpolate(
            endPoint.GlobalPosition - GlobalPosition,
            (float) i / NodeCount
          ));
      }
    }


    public void Simulate()
    {
      if (_segments == null) GeneratePoints();

      for (var i = 0; i < _segments.Length; i++)
      {
        // verlet integration or whatever idk im not smart
        Segment segment = _segments[i];

        Vector2 velocity = segment.Pos - segment.OldPos;
        segment.OldPos = segment.Pos;

        segment.Pos += velocity;

        // Gravity is multiplied by .1f because the editor has an epsilon of .01f so its hard
        // to get fine detailed control
        segment.Pos.y += Gravity * GetProcessDeltaTime() * .01f;
        _segments[i] = segment;
      }

      ApplyConstraints();
    }

    public void ApplyConstraints()
    {
      var endPoint = GetNodeOrNull<Node2D>(AnchorPath);
      if (endPoint is null) return;

      int lastIndex = _segments.Length - 1;
      float idealDist = ToLocal(endPoint.GlobalPosition).Length() / NodeCount + AdditionalBinding;

      for (var i = 0; i < lastIndex; i++)
      {
        Segment segment = _segments[i], neighbor = _segments[i + 1];

        // Calc correction between points for that spicy rope behaviour
        float distance = segment.Pos.DistanceTo(neighbor.Pos);
        float error = Mathf.Abs(distance - idealDist);
        Vector2 direction = Vector2.Zero;

        if (distance > idealDist)
          direction = (segment.Pos - neighbor.Pos).Normalized();
        else if (distance > idealDist)
          direction = (neighbor.Pos - segment.Pos).Normalized();

        // Apply correction to both points
        Vector2 correction = direction * error;
        segment.Pos -= correction * Tension;
        neighbor.Pos += correction * Tension;

        _segments[i] = segment;
        _segments[i + 1] = neighbor;
      }

      // Single point constraints

      // Pins first point to specified node or origin of this node
      Segment first = _segments[0];
      first.Pos = Vector2.Zero;
      _segments[0] = first;

      if (!ConstrainEndPoint) return;
      Segment last = _segments[lastIndex];
      last.Pos = endPoint.GlobalPosition - GlobalPosition;
      _segments[lastIndex] = last;
    }

    public override void _PhysicsProcess(float delta)
    {
      // Refuse to simulate unless an endpoint is set
      if (GetNodeOrNull<Node2D>(AnchorPath) is null) return;

#if DEBUG
      if (!Engine.EditorHint)
#endif
        Simulate();

      if (_segments is null) return;

      // Renders points
      Points = (from segment in _segments select segment.Pos).ToArray();
    }

    internal struct Segment
    {
      public Vector2 Pos, OldPos;
      public Segment(Vector2 pos) => Pos = OldPos = pos;
    }
  }
}