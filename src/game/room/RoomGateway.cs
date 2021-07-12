using System;
using Godot;
using Parry2.game.actors.player;
using Parry2.managers.save;

namespace Parry2.game.room
{
    public class RoomGateway : Area2D
    {
        public const string GatewayGroup = "RoomGateway";
        [Export] public float Cooldown = .63f;
        [Export] public GateDirection Direction;
        [Export] public string TargetRoomName = string.Empty, TargetGate = string.Empty;

        [Export] public bool Active = true;

        Timer _cooldownTimer, _cutsceneTimer;
        Vector2 _dirV;
        PlayerShroom _player;

        public override void _Ready()
        {
            _cooldownTimer = new Timer();
            AddChild(_cooldownTimer);
            _cooldownTimer.Connect("timeout", _cooldownTimer, "queue_free");
            _cooldownTimer.Start(Cooldown);
            SetProcess(false);
        }

        public void CutsceneTimeout()
        {
            _cutsceneTimer.QueueFree();
            SetProcess(false);
            _player.ControlActive = true;
        }

        public override void _Process(float delta)
        {
            _player.Velocity = new Vector2((_player.MaxSpeed * -_dirV).x, _player.Velocity.y);
        }

        public void EnteredCutscene(PlayerShroom player)
        {
            _player = player;
            _dirV = GateDirectionToVector2(Direction);

            player.GlobalPosition = GlobalPosition;

            void Horizontal()
            {
                SetProcess(true);
                _cutsceneTimer = new Timer();
                AddChild(_cutsceneTimer);
                _cooldownTimer.Connect("timeout", this, nameof(CutsceneTimeout));
                _cutsceneTimer.Start(Cooldown);

                player.ControlActive = false;
            }

            switch (Direction)
            {
                case GateDirection.Left:
                case GateDirection.Right:
                    Horizontal();
                    break;

                case GateDirection.Up:
                case GateDirection.Down:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnEntered(Node body)
        {
            if (!Active) return;
            if (_cooldownTimer?.IsInsideTree() ?? false) return;
            if (body is not PlayerShroom player) return;
            _player = player;

            SaveManager.Save();
            GameplayScene.LoadRoom(TargetRoomName, TargetGate);
        }

        static Vector2 GateDirectionToVector2(GateDirection direction) => direction switch
        {
            GateDirection.Right => Vector2.Right,
            GateDirection.Left => Vector2.Left,
            GateDirection.Down => Vector2.Down,
            GateDirection.Up => Vector2.Up,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, string.Empty)
        };

#nullable disable
        public enum GateDirection
        {
            Right,
            Left,
            Up,
            Down
        }
    }
}