using Godot;
using Parry2.game.rooms;
using Parry2.managers.save;

namespace Parry2.game.room
{
    public class RoomGateway : Area2D
    {
        [Export] public float Cooldown = .5f;
        [Export] public GateDirection Direction;
        [Export] public string TargetRoomName;

        [Export]
        public bool Active
        {
            set => _active = value;
            get => _active && RoomList.IsValidRoomName(TargetRoomName) && _timer == null;
        }

        bool _active = true;
        Timer _timer;

        public override void _Ready()
        {
            _timer = new Timer();
            AddChild(_timer);
            _timer.Connect("timeout", this, nameof(Timeout));
            _timer.Start(Cooldown);
        }

        public void Timeout()
        {
            _timer.QueueFree();
            _timer = null;
        }

        public void OnEntered(Node body)
        {
            if (!Active) return;

            SaveManager.Save();
            GameplayScene.LoadRoom(TargetRoomName);
        }

        public enum GateDirection
        {
            Right,
            Left,
            Up,
            Down
        }
    }
}