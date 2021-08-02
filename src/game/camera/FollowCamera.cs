using System;
using System.Reactive.Linq;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Nita.addons.godotrx;
using Nita.utils;
using Array = Godot.Collections.Array;

namespace Nita.game.camera
{
  public class FollowCamera : Area2D
  {
    [Node("CameraContainer/Camera")] readonly Camera2D _camera = null;
    [Node("CameraContainer")] readonly Node2D _cameraContainer = null;
    [Node("CollisionShape2D")] readonly CollisionShape2D _collisionShape = null;
    bool _isRespawning;

    Vector2 _targetPos;

    [Export] public float SmoothingSpeed = .1f, MaxSmoothVel;

    [Export] public NodePath TargetPath { set; get; }
    public Node2D Target => GetNodeOrNull<Node2D>(TargetPath);

    public override void _Ready()
    {
      this.Wire();

      this.Dispatch(() => { _isRespawning = false; }, .1f);

      this.OnProcess()
          .Where(_ => _isRespawning)
          .Subscribe(_ => GlobalPosition = _targetPos)
          .DisposeWith(this);

      this
          .OnProcess()
          .Where(_ => !_isRespawning)
          .Subscribe(Travel)
          .DisposeWith(this);
      // _camera.Zoom = Vector2.One;
    }

    void Travel(float delta)
    {
      // _camera.Zoom = Vector2.One;

      _collisionShape.GlobalPosition = Target.GlobalPosition;

      float smoothSpeedDelta = 1 - Mathf.Pow(SmoothingSpeed, delta);

      // Smooths position to target
      GlobalPosition = GlobalPosition.LinearInterpolate(_targetPos, smoothSpeedDelta);
      _cameraContainer.GlobalPosition = GlobalPosition.Round();
    }

    public override void _PhysicsProcess(float delta)
    {
      Array areas = GetOverlappingAreas();


      // camera.DragMarginHEnabled = camera.DragMarginVEnabled = false;
      if (areas.Count > 0)
      {
        _controlByArea((CameraControlArea) areas[0]);
        return;
      }

      _camera.DragMarginHEnabled = _camera.DragMarginVEnabled = true;

      _targetPos = Target.GlobalPosition;
    }

    void _controlByArea(CameraControlArea area)
    {
      // GlobalRotation = Mathf.LerpAngle(GlobalRotation, 0, .1f);
      _targetPos = area.GetNodeOrNull<Node2D>(area.Point)?.GlobalPosition ??
                   Target.GlobalPosition;

      _camera.DragMarginHEnabled = _camera.DragMarginVEnabled = false;
    }
  }
}