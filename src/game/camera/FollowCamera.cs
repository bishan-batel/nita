using Godot;
using Godot.Collections;
using Parry2.game.actors.player;
using Parry2.utils;

namespace Parry2.game.camera
{
    public class FollowCamera : Area2D
    {
        Margin4 _dragMargin, _targetMargin;

        Vector2 _targetPos, _targetZoom;
        [Export] public float SmoothingSpeed, MaxSmoothVel, ZoomRoundAmt;

        [Export]
        public NodePath TargetPath
        {
            set
            {
                _targetPath = value;
                if (!IsInsideTree()) return;
            }
            get => _targetPath;
        }

        [Export] public Vector2 Zoom = new Vector2(.9f, .9f);

        (Timer timer, bool respawnDone) _timerData;
        NodePath _targetPath;

        public override void _Ready()
        {
            _timerData.timer = new Timer();
            AddChild(_timerData.timer);
            _timerData.timer.Start(.01f);
            _timerData.timer.Connect("timeout", this, nameof(Timeout));
        }

        public void Timeout()
        {
            RemoveChild(_timerData.timer);
            _timerData.respawnDone = true;

            GlobalPosition =
                _targetPos =
                    GetNode<Node2D>(TargetPath).GlobalPosition;

            _targetZoom = Zoom;

            var camera = GetNode<Camera2D>("Camera");
            _dragMargin = new Margin4(camera);
        }

        public override void _Process(float delta)
        {
            if (!_timerData.respawnDone) return;

            GetNode<CollisionShape2D>("CollisionShape2D")
                .GlobalPosition = GetNode<Node2D>(TargetPath).GlobalPosition;

            var camera = GetNode<Camera2D>("Camera");

            // Smooths position to target
            GlobalPosition = GlobalPosition.LinearInterpolate(_targetPos, SmoothingSpeed).Round();

            // Smooths camera to target
            camera.Zoom = camera.Zoom.LinearInterpolate(_targetZoom, SmoothingSpeed);
            camera.Zoom = (camera.Zoom / ZoomRoundAmt).Round() * ZoomRoundAmt;
            if (camera.Zoom == Vector2.Zero)
                camera.Zoom = new Vector2(ZoomRoundAmt, ZoomRoundAmt);

            // Smooths margin to target
            new Margin4(camera)
                .Lerp(_targetMargin, SmoothingSpeed)
                .Assign(camera);
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
        }

        void _controlByArea(CameraControlArea area)
        {
            _targetPos = area.GetNodeOrNull<Node2D>(area.Point)?.GlobalPosition ??
                         GetNode<Node2D>(TargetPath).GlobalPosition;

            _targetMargin = new Margin4(0f, 0f, 0f, 0f);
            if (area.CameraZoom != Vector2.Zero)
                _targetZoom = area.CameraZoom;
        }
    }
}