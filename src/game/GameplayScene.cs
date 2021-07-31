using System;
using System.Linq;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Nita.addons.godotrx;
using Nita.debug;
using Nita.game.room;
using Nita.managers.game;
using Nita.managers.save;

namespace Nita.game
{
  public class GameplayScene : GameState
  {
    public static GameplayScene Singleton;
    public static string EnteredGate;

    /// <summary>
    /// Container for rooms to be switched in and out of
    /// </summary>
    [Node("RoomViewContainer/RoomContainer")]
    public readonly Viewport RoomContainer = null;

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
          ?.Instance<Room>(PackedScene.GenEditState.Instance);
#else
                ?.Instance<Room>();
#endif

      // Clears Container
      RoomContainer
          .GetChildren()
          .Cast<Node>()
          .ToList()
          .ForEach(child => child.Free());

      RoomContainer.AddChild(CurrentRoom);
      AddCommands();
    }


    /// <summary>
    /// Loads specified room from specified ID, will only work if game is already in a room / in gameplay state
    /// </summary>
    /// <param name="roomName">Room string ID</param>
    /// <param name="save">Should save the old room before loading</param>
    /// <param name="gateway">Name of gateway to enter</param>
    public static void LoadRoom(string roomName, bool save = true, string gateway = null)
    {
      if (GameStateManager.CurrentState is not GameplayScene)
        throw new WrongGamestateException(typeof(GameplayScene));

      // Singleton.DebugPrint($"Loading into {roomName} through gateway {gateway ?? "[default]"}");

      if (save) SaveManager.Save();

      CurrentRoom
          .OnTreeExiting()
          .Subscribe(_ =>
          {
            EnteredGate = gateway;
            CurrentRoom = RoomList.GetChapterScene(roomName).Instance<Room>(PackedScene.GenEditState.Instance);
            Singleton.RoomContainer.AddChild(CurrentRoom);
          })
          .DisposeWith(Singleton);

      CurrentRoom.QueueFree();
    }

    /// <summary>
    /// Used to load a room using the current save file, intended use is transition from other game states into gameplay
    /// </summary>
    public static void LoadFromSave() => Global
        .Singleton
        .GetTree()
        .ChangeSceneTo(PackedScene);

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