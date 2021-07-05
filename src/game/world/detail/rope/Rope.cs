using Godot;

namespace Parry2.game.world.detail.rope
{
    [Tool]
    public class Rope : Line2D
    {
        int _pointCount;
        Segment[] _segments;
        [Export] public float AdditionalBinding, Gravity = 1f;
        [Export] public bool ConstrainEndPoint = true, SimulateInEditor = true, Active = true;

        [Export] public uint ConstraintDetail = 50;

        [Export] public NodePath EndPosition;
        [Export] public NodePath StartPosition { set; get; }

        [Export]
        public int PointCount
        {
            set
            {
                _pointCount = value;
                GeneratePoints();
            }
            get => _pointCount;
        }

        public override void _Ready()
        {
            GeneratePoints();
        }

        public void GeneratePoints()
        {
            if (!IsInsideTree() || !Active) return;
            var endPoint = GetNodeOrNull<Node2D>(EndPosition ?? GetPath());
            if (endPoint == null) return;

            _segments = new Segment[PointCount + 1];


            var startPosN = GetNodeOrNull<Node2D>(StartPosition)?.GlobalPosition;
            var startPos = (Vector2) (startPosN == null ? Vector2.Zero : startPosN - GlobalPosition);

            for (var i = 0; i <= PointCount; i++)
                _segments[i] = new Segment(
                    startPos.LinearInterpolate(
                        endPoint.GlobalPosition - GlobalPosition,
                        (float) i / PointCount
                    ));
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

            for (var i = 0; i < ConstraintDetail; i++)
                ApplyConstraints();
        }

        public void ApplyConstraints()
        {
            var endPoint = GetNodeOrNull<Node2D>(EndPosition);
            if (endPoint == null) return;

            float idealDist = GlobalPosition.DistanceTo(endPoint.GlobalPosition) / PointCount + AdditionalBinding;

            for (var i = 0; i < _segments.Length - 1; i++)
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
                segment.Pos -= correction * .5f;
                neighbor.Pos += correction * .5f;

                _segments[i] = segment;
                _segments[i + 1] = neighbor;
            }

            // Single point constraints

            // Pins first point to specified node or origin of this node
            Segment first = _segments[0];
            // first.Pos = GetGlobalMousePosition() - GlobalPosition;
            var startPos = GetNodeOrNull<Node2D>(StartPosition)?.GlobalPosition;
            first.Pos = (Vector2) (startPos == null ? Vector2.Zero : startPos - GlobalPosition);
            _segments[0] = first;

            if (!ConstrainEndPoint) return;
            Segment last = _segments[_segments.Length - 1];
            last.Pos = endPoint.GlobalPosition - GlobalPosition;
            _segments[_segments.Length - 1] = last;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (!Active) return;
            // Refuse to simulate unless an endpoint is set
            if (GetNodeOrNull<Node2D>(EndPosition) == null) return;

            if (!Engine.EditorHint || SimulateInEditor)
                Simulate();

            // Renders points
            ClearPoints();
            foreach (Segment segment in _segments)
                AddPoint(segment.Pos);
        }

        internal struct Segment
        {
            public Vector2 Pos, OldPos;

            public Segment(Vector2 pos)
            {
                Pos = OldPos = pos;
            }
        }
    }
}