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
	(float top, float right, float down, float left) _originalMargin;

	[Export] public float SmoothingSpeed = .1f, MaxSmoothVel;
	[Export] public NodePath TargetPath { set; get; }
	public Node2D Target => GetNodeOrNull<Node2D>(TargetPath);

	public override void _Ready()
	{
	  this.Wire();
	  _isRespawning = true;
	  _originalMargin = (_camera.DragMarginTop, _camera.DragMarginRight, _camera.DragMarginBottom, _camera.DragMarginLeft);

	  this.Dispatch(() => { _isRespawning = false; }, .2f);

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
		_controlByArea((CameraControlArea)areas[0]);
		return;
	  }

	  _camera.DragMarginHEnabled = _camera.DragMarginVEnabled = true;

	  _targetPos = Target.GlobalPosition;

	  // Smooth drag margin (pain)
	  _lerpDragMargin(_originalMargin.top, _originalMargin.right, _originalMargin.left, _originalMargin.down);
	}

	void _controlByArea(CameraControlArea area)
	{
	  // GlobalRotation = Mathf.LerpAngle(GlobalRotation, 0, .1f);
	  _targetPos = area.GetNodeOrNull<Node2D>(area.Point)?.GlobalPosition ??
				   Target.GlobalPosition;
	  _targetPos -= _cameraContainer.ToLocal(_camera.GlobalPosition);

	  _lerpDragMargin(0, 0, 0, 0);
	}

	void _lerpDragMargin(float top, float right, float left, float bottom)
	{
	  const float snap = .1f;
	  float smoothSpeedDelta = 1 - Mathf.Pow(SmoothingSpeed, GetProcessDeltaTime());
	  _camera.DragMarginBottom = _round(Mathf.Lerp(_camera.DragMarginBottom, bottom, smoothSpeedDelta), snap);
	  _camera.DragMarginTop = _round(Mathf.Lerp(_camera.DragMarginTop, top, smoothSpeedDelta), snap);
	  _camera.DragMarginLeft = _round(Mathf.Lerp(_camera.DragMarginLeft, left, smoothSpeedDelta), snap);
	  _camera.DragMarginRight = _round(Mathf.Lerp(_camera.DragMarginRight, right, smoothSpeedDelta), snap);
	}

	float _round(float num, float r) => Mathf.Round(num / r) * r;
  }
}
