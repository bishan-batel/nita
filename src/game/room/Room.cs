using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;
using Nita.addons.godotrx;
using Nita.editor;
using Nita.game.world.actors.player;
using Nita.game.world.objects.checkpoint;
using Nita.managers.save;
using Nita.utils;
using Environment = Godot.Environment;

namespace Nita.game.room
{
  public class Room : Node
  {
    [Export] public Environment WorldEnviorment;
    [Export] public NodePath DefaultGateway;

    [Export]
    public NodePath PlayerPath
    {
      set
      {
        _playerPath = value;
        if (!IsInsideTree()) return;
        if (value is null) return;
        Player = GetNode<PlayerShroom>(_playerPath);
      }
      get => _playerPath;
    }

    [Export] public string RoomName = string.Empty;
    public PlayerShroom Player { private set; get; }
    NodePath _playerPath;


    public CheckpointManagerNode CheckpointManager { private set; get; }

    public override async void _Ready()
    {
      PlayerPath = PlayerPath;
      // TODO fix checkpoints

#if DEBUG
      // Used when debugging to bypass having to go through menu each time
      if (GetTree().CurrentScene == this)
      {
        GetTree().ChangeSceneTo(GameplayScene.PackedScene);
        GameplayScene.CurrentRoom = (Room) Duplicate();
        return;
      }
#endif

      // Checks if room is valid
      if (!RoomList.IsValidRoomName(RoomName))
        throw new Exception("Unauthorized name");

      // Adds checkpoint manager
      CheckpointManager = new CheckpointManagerNode();
      AddChild(CheckpointManager, true);

      // Awaits frame to let checkpoint manager process
      await this.WaitNextIdleFrame();


      LoadFromSave(SaveManager.CurrentSaveFile);

      await this.WaitNextIdleFrame();

      var player = GetNode<PlayerShroom>(PlayerPath);

      // Attempts to find entered gateway in scene
      RoomGateway gateway = GetTree()
          .GetNodesInGroup(RoomGateway.GatewayGroup)
          .Cast<RoomGateway>()
          .FirstOrDefault(gate => gate.Name == GameplayScene.EnteredGate);

      // If gateway doesn't exist then try to load from checkpoint
      if (gateway is not null)
      {
        this.DebugPrint($"Entering gateway [{gateway.Name}]");
        gateway.EnteredCutscene(player);
      }
      else if (CheckpointManager.ClaimedPath is not null)
      {
        Vector2 location = CheckpointManager.GetSpawnLocation();
        player.GlobalPosition = location;
      }
      // If checkpoint or entered gateway doesn't exist, go through the default
      else
      {
        var @default = GetNodeOrNull<RoomGateway>(DefaultGateway);
        @default?.EnteredCutscene(player);
        this.DebugPrint($"Entering room through default gateway [{@default?.Name ?? "null"}]");
      }

      // Applies world enviorment
      GameplayScene
          .Singleton
          .GetNode<WorldEnvironment>("Env")
          .Environment = WorldEnviorment ?? new Environment();
    }

    public void OnDeath()
    {
      // GetTree().ReloadCurrentScene();
      SaveManager.Save();
      GameplayScene.LoadRoom(RoomName);
    }

#if DEBUG
    public override void _Process(float delta)
    {
      if (Input.IsActionJustPressed("debug_load_from_save"))
        GetTree().ReloadCurrentScene();
    }
#endif

    public void LoadFromSave(SaveFile saveFile = null)
    {
      saveFile ??= SaveManager.CurrentSaveFile;
      if (!saveFile.RoomData.ContainsKey(RoomName)) return;

      // Room-bound Persistent Node Loading
      var roomSave = saveFile.RoomData[RoomName];
      if (roomSave is null) return;

      this.DebugPrint($"Loading {RoomName} from save file");

      roomSave
          .Keys
          .ToList()
          .ForEach(key =>
          {
            var persistant = GetNodeOrNull(key) as IPersistant;
            persistant?.LoadFrom(roomSave[key]);
          });

      // TODO Implement global loading

      ObjectOrder.OrganizeLayersInTree(GetTree());
    }

    /// <summary>
    /// Saves data to save file in memory, will not save to filesystem
    /// </summary>
    /// <param name="saveFile">Save file to write into, defaults to the currently loaded save file</param>
    /// <exception cref="Exception"></exception>
    public void SaveData(SaveFile saveFile = null)
    {
      this.DebugPrint("Getting Room Data...");

      saveFile ??= SaveManager.CurrentSaveFile;

      saveFile.CurrentRoomName = RoomName;

      // Room-bound Persistent Node Saving
      var roomSaves = new Dictionary<string, ISerializable>();
      GetTree()
          .GetNodesInGroup(SaveManager.PersistGroup)
          .Cast<Node>()
          .ToList()
          .ForEach(node =>
          {
            if (node is not IPersistant persistant)
              throw new GroupInterfaceException(node, typeof(IPersistant));

            // this.DebugPrint($"\tSaved data from {node.Name}");
            roomSaves[GetPathTo(node)] = persistant.Save();
          });

      // Saves room data
      saveFile.RoomData[RoomName] = roomSaves;

      // Saves global persistant data
      GetTree()
          .GetNodesInGroup(SaveManager.GlobalPersistGroup)
          .Cast<Node>()
          .ToList()
          .ForEach(node =>
          {
            if (node is not IGlobalPersistant globalPersistant)
              throw new GroupInterfaceException(node, typeof(IGlobalPersistant));

            ISerializable nodeSave = globalPersistant.GlobalSave();
            saveFile.GlobalData[globalPersistant.UniqueName] = nodeSave;
          });
    }
  }
}