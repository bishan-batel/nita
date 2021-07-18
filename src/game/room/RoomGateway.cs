using System;
using Godot;
using Parry2.managers.save;
using Parry2.utils;
using PlayerShroom = Parry2.game.world.actors.player.PlayerShroom;

namespace Parry2.game.room
{
    public class RoomGateway : Area2D
    {
        public enum GateDirection
        {
            Right,
            Left,
            Up,
            Down
        }

        public const string GatewayGroup = "RoomGateway";
        Vector2 _cutsceneDir;


        bool _onCooldown = true;
        PlayerShroom _player;

        [Export] public bool Active = true;

        // Exports
        [Export] public float Cooldown = .63f,
            CutsceneWalkTime = .5f;

        [Export] public GateDirection Direction;

        [Export] public string TargetRoomName = string.Empty,
            TargetGate = string.Empty;

        public override void _Ready()
        {
            Timeout.Dispatch(() => _onCooldown = false, Cooldown);
            SetProcess(false);
        }


        public override void _Process(float delta)
        {
            _player.Velocity = new Vector2((_player.MaxSpeed * -_cutsceneDir).x, _player.Velocity.y);
        }

        public void EnteredCutscene(PlayerShroom player)
        {
            _player = player;
            _cutsceneDir = GateDirectionToVector2(Direction);

            player.GlobalPosition = GlobalPosition;


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

        void Horizontal()
        {
            SetProcess(true);
            _player.ControlActive = false;

            Timeout.Dispatch(() =>
            {
                SetProcess(false);
                _player.ControlActive = true;
            }, CutsceneWalkTime);
        }

        public void OnEntered(Node body)
        {
            if (!Active || _onCooldown) return;
            if (body is not PlayerShroom player) return;
            _player = player;

            // Notify checkpoint manager that we left the room
            Room.CheckpointManager.OnLeftRoom();

            SaveManager.Save();
            GameplayScene.LoadRoom(TargetRoomName, TargetGate);
        }

        static Vector2 GateDirectionToVector2(GateDirection direction)
        {
            return direction switch
            {
                GateDirection.Right => Vector2.Right,
                GateDirection.Left => Vector2.Left,
                GateDirection.Down => Vector2.Down,
                GateDirection.Up => Vector2.Up,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, string.Empty)
            };
        }
    }
}