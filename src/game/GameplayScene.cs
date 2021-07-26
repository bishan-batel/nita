using System;
using System.Linq;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using GodotRx;
using Parry2.debug;
using Parry2.game.room;
using Parry2.managers.game;
using Parry2.managers.save;
using Parry2.utils;

namespace Parry2.game
{
  public class GameplayScene : GameState
  {
    public static GameplayScene Singleton;
    public static string EnteredGate;

    [Node("ChapterViewContainer/ChapterContainer")]
    public readonly Viewport ChapterContainer = null;

    [Node("UI/DialogueManager")] public readonly GameDialogueManager DialogueManager = null;
    [Export] public string DefaultChapterName = "test_room";

    //  I would use default parameters to have just one constructor, but godot
    // seems to just crash when trying to instance it
    // (maybe an issue to do with connecting the script to the node?)
    public GameplayScene() : this(null)
    {
    }

    public GameplayScene(Room room) : base(nameof(GameplayScene))
    {
      CurrentRoom = room;
      Singleton = this;
    }

    public static PackedScene PackedScene =>
        ResourceLoader.Load<PackedScene>("res://src/game/GameplayScene.tscn");


    public static Room CurrentRoom { set; get; }

    public override void _Ready()
    {
      base._Ready();
      this.Wire();

      CurrentRoom ??= SaveManager
          .CurrentSaveFile
          ?.CurrentRoom
#if DEBUG
          ?.Instance(PackedScene.GenEditState.Instance) as Room;
#else
                ?.Instance() as Room;
#endif

      // Clears Container
      ChapterContainer
          .GetChildren()
          .Cast<Node>()
          .ToList()
          .ForEach(child => child.Free());

      ChapterContainer.AddChild(CurrentRoom);
      AddCommands();
    }


    public static void LoadRoom(string roomName, bool save = true, string gateway = null)
    {
      // Singleton.DebugPrint($"Loading into {roomName} through gateway {gateway ?? "[default]"}");

      if (save)
        SaveManager.Save();

      CurrentRoom
          .OnTreeExiting()
          .Subscribe(_ =>
          {
            EnteredGate = gateway;
            CurrentRoom = RoomList.GetChapterScene(roomName).Instance<Room>(PackedScene.GenEditState.Instance);
            Singleton.ChapterContainer.AddChild(CurrentRoom);
          }).DisposeWith(Singleton);

      CurrentRoom.QueueFree();


      // Global
      //     .Singleton
      //     .GetTree()
      //     .ChangeSceneTo(RoomList.GetChapterScene(roomName));
    }

    public static void LoadRoomFromCurrentScene(Room room)
    {
      room.DebugPrint($"Changing scene from {nameof(Room)} to {nameof(GameplayScene)}");

      room
          .GetTree()
          .ChangeSceneTo(PackedScene);

      // stores in static variable as a mock constructor argument
      CurrentRoom = (Room) room.Duplicate();

      // I'm like 50% sure this queue free is not required because you are changing the scene
      // but I don't want to risk a memory leak so I'm gonna leave it in here
      room.QueueFree();
    }

    public static void LoadFromSave(SaveFile file) => LoadRoom(file.CurrentRoomName);

    public void AddCommands()
    {
      this.AddCommand(
        "room",
        "Changes room to specified ID",
        nameof(CmdRoom),
        ("room_name", Variant.Type.String)
      );
    }

    void CmdRoom(string roomName)
    {
      if (!RoomList.IsValidRoomName(roomName))
      {
        GConsole.WriteLine($"[color=red]{roomName} is invalid");
        return;
      }

      LoadRoom(roomName);
    }
  }
}