using Godot;
using Godot.Collections;
using Parry2.utils;

namespace Parry2.game.camera
{
    public class FollowCamera : Area2D
    {
        CMargin4 _dragMargin, _targetMargin;

        Vector2 _targetPos, _targetZoom;
        bool _respawnDone;

        [Export] public float SmoothingSpeed = .1f, MaxSmoothVel, ZoomRoundAmt;

        [Export] public Vector2 Zoom = new(.9f, .9f);

        [Export] public NodePath TargetPath { set; get; }

        public override void _Ready()
        {
            Timeout.Dispatch(() =>
            {
                _dragMargin = new CMargin4(GetNode<Camera2D>("Camera"));
                _respawnDone = true;
            }, .1f);
        }

        public override void _Process(float delta)
        {
            if (!_respawnDone)
            {
                GlobalPosition = _targetPos;
                _targetZoom = Zoom;
                return;
            }

            GetNode<CollisionShape2D>("CollisionShape2D")
                .GlobalPosition = GetNode<Node2D>(TargetPath).GlobalPosition;

            var camera = GetNode<Camera2D>("Camera");

            float smoothSpeedDelta = 1 - Mathf.Pow(SmoothingSpeed, delta);

            // Smooths position to target
            GlobalPosition = GlobalPosition.LinearInterpolate(_targetPos, smoothSpeedDelta).Round();

            // Smooths camera to target
            camera.Zoom = camera.Zoom.LinearInterpolate(_targetZoom, smoothSpeedDelta);
            camera.Zoom = (camera.Zoom / ZoomRoundAmt).Round() * ZoomRoundAmt;
            if (camera.Zoom == Vector2.Zero)
                camera.Zoom = new Vector2(ZoomRoundAmt, ZoomRoundAmt);

            // Smooths margin to target
            new CMargin4(camera)
                .Lerp(_targetMargin, smoothSpeedDelta)
                .AssignToDragMargin(camera);
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

            _targetMargin = _dragMargin;

            _targetPos = GetNode<Node2D>(TargetPath).GlobalPosition;
            _targetZoom = Zoom;
            // GlobalRotation = GetNode<Node2D>(TargetPath).GlobalRotation;
        }

        void _controlByArea(CameraControlArea area)
        {
            // GlobalRotation = Mathf.LerpAngle(GlobalRotation, 0, .1f);
            _targetPos = area.GetNodeOrNull<Node2D>(area.Point)?.GlobalPosition ??
                         GetNode<Node2D>(TargetPath).GlobalPosition;

            _targetMargin = new CMargin4(0f, 0f, 0f, 0f);
            if (area.CameraZoom != Vector2.Zero)
                _targetZoom = area.CameraZoom;
        }
    }
}