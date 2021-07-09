using System;
using Godot;
using Godot.Collections;

namespace Parry2.game.rooms
{
    public static class RoomList
    {
        public const string RoomsFolder = "res://src/game/rooms/";

        static string Format(string path) => RoomsFolder.PlusFile(path);

        // List of all valid room names, need to manually sync with editor values for each node
        public static readonly Dictionary<string, string> Rooms = new()
        {
            {"test_room", Format("TestRoom.tscn")},
        };

        public static PackedScene GetChapterScene(Room room) =>
            GetChapterScene(GetName(room.RoomName));

        public static PackedScene GetChapterScene(string name) =>
            ResourceLoader.Load<PackedScene>(Rooms[GetName(name)]);

        public static bool IsValidRoomName(string name) => Rooms.ContainsKey(name);

        // Used to throw exceptions if there are any unauthorized room names trying to be used
        // (for just simpler management)
        public static string GetName(string testRoom)
        {
            if (!IsValidRoomName(testRoom)) throw new Exception($"{testRoom} is not a valid room name");
            return testRoom;
        }
    }
}