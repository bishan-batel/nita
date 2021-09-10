using System;
using Godot;
using Godot.Collections;

namespace Nita.game.room
{
  public static class RoomList
  {
    public const string RoomsFolder = "res://src/game/room/rooms";

    // List of all valid room names, need to manually sync with editor values for each node
    public static readonly Dictionary<string, string> Rooms = new()
    {
      // DEBUG
      {
        "test_room", _format(@"TestRoom.tscn")
      },

      // GARDEN
      {
        "garden_entrance", _format(@"garden/GardenEntrance.tscn")
      }
    };

    static string _format(string path) => RoomsFolder.PlusFile(path);

    public static PackedScene GetChapterScene(Room room) => GetChapterScene(GetName(room.RoomName));

    public static PackedScene GetChapterScene(string name) =>
        ResourceLoader.Load<PackedScene>(Rooms[GetName(name)]);

    public static bool IsValidRoomName(string name) => Rooms.ContainsKey(name);

    // Used to throw exceptions if there are any unauthorized room names trying to be used
    // (for just simpler management in development)
    public static string GetName(string testRoom)
    {
      if (!IsValidRoomName(testRoom))
        throw new Exception($"{testRoom} is not a valid room name");
      return testRoom;
    }
  }
}