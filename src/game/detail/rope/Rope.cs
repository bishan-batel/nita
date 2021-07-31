using Godot;
using GodotRx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using GodotOnReady.Attributes;
using GodotRx.Internal;
using Parry2.utils;

// ReSharper disable UnassignedField.Global

namespace Parry2.game.detail.rope
{
  [Tool]
  public partial class Rope : Line2D
  {
    [Export] public bool Active { set; get; } = true;

    // Editor Simulation parameters
    [Export] public NodePath AnchorPath;
    public Node2D Anchor => IsInsideTree() ? GetNodeOrNull<Node2D>(AnchorPath) : null;


    [Export]
    public int NodeCount
    {
      set
      {
        _count = value;
        Generate();
      }
      get => _count;
    }

    [Export] public uint ConstrainMultiple = 50;

    [Export] public float Gravity { set; get; }

    [Export(PropertyHint.Range, "0,1")] public float Tension { set; get; } = .5f;

    Segment[] _segments;

    // Calculated variables

    float _desiredDist;
    int _count;

    [OnReady]
    public void OnReady()
    {
      Generate();
      this
          .OnProcess()
#if DEBUG
          .Where(_ => !Engine.EditorHint)
#endif
          .Where(_ => Active)
          .Where(_ => _segments is not null && _segments.Length > 0)
          .Subscribe(Simulate)
          .DisposeWith(this);
    }

    /// <summary>
    /// Generates line points from origin to anchor position
    /// </summary>
    public void Generate()
    {
      if (Anchor is null) return;

      Vector2 localAnchorPos = ToLocal(Anchor.GlobalPosition);
      float ropeLength = Anchor.DistanceTo(this);

      _desiredDist = ropeLength / NodeCount;

      _segments = GD.Range(0, NodeCount)
          .Select(i =>
          {
            float percent = (float) i / NodeCount;
            Vector2 pos = Vector2.Zero.LinearInterpolate(localAnchorPos, percent);
            return new Segment(pos);
          })
          .ToArray();

      Render();
    }

    public void Render()
    {
      Points = (from segment in _segments select segment.Pos).ToArray();
#if DEBUG
      if (Engine.EditorHint)
        Update();
#endif
    }

    public void Simulate(float delta)
    {
      _segments = _segments
          .Select(segment =>
          {
            // Updates position with calculated velocity
            Vector2 temp = segment.Pos;
            segment.Pos += segment.Vel;
            segment.OldPos = temp;
            return segment;
          })
          .ToArray();

      for (var i = 0; i < ConstrainMultiple; i++) ConstrainPoints();

      Render();
    }

    public void ConstrainPoints()
    {
      Enumerable.Range(0, _segments.Length - 1)
          .ToList()
          .SafeForEach(i =>
          {
            Segment first = _segments[i], second = _segments[i + 1];

            Vector2 diff = second.Pos - first.Pos;
            Vector2 correction = diff * Tension;

            first.Pos += correction;
            second.Pos += correction;

            // Reassigns to array
            _segments[i] = first;
            _segments[i + 1] = second;
          });
    }

#if DEBUG
    public override void _Draw()
    {
      if (Anchor is not null)
        DrawLine(GlobalPosition, Anchor.GlobalPosition, Colors.Tomato, 4f);
    }
#endif

    struct Segment
    {
      public Vector2 Vel => Pos - OldPos;
      public Vector2 Pos, OldPos;

      public Segment(Vector2 pos) => Pos = OldPos = pos;
    }
  }
}