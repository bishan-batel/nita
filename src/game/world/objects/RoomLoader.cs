using System;
using Godot;
using Parry2.game.room;

namespace Parry2.game.world.objects
{
    // TODO implement in godot editor
    public class RoomLoader
    {
        [Export] public string RoomName { set; get; } = "";
        [Export] public bool Active { set; get; }

        public void _trigger()
        {
            if (RoomName == "") throw new Exception("Target Room is not set");
            if (RoomList.IsValidRoomName(RoomName)) throw new Exception("Target Room has invalid name");
        }

        [Signal]
        public delegate void OnRoomChange();
    }
}