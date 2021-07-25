using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;
using GodotRx;
using Parry2.editor;
using Parry2.game.world.actors.player;
using Parry2.game.world.objects.checkpoint;
using Parry2.managers.save;
using Parry2.utils;
using Environment = Godot.Environment;

namespace Parry2.game.room
{
  public class Room : Node
  {
    [Export] public Environment WorldEnviorment;
    [Export] public NodePath DefaultGateway;
    [Export] public NodePath Player;
    [Export] public string RoomName = string.Empty;


    public CheckpointManagerNode CheckpointManager { private set; get; }

    public override async void _Ready()
    {
      // TODO fix checkpoints

      // Used when debugging to bypass having to go through menu each time
      if (GetTree().CurrentScene == this)
      {
        GameplayScene.LoadRoomFromCurrentScene(this);
        return;
      }

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

      var player = GetNode<PlayerShroom>(Player);

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
          .ConvertAll(nodePath => (GetNodeOrNull<IPersistant>(nodePath), nodePath))
          .FindAll(node => node.Item1 is not null)
          .ForEach(node =>
          {
            (IPersistant persistant, string nodePath) = node;
            // this.DebugPrint($"\tLoading {nodePath}");
            persistant.LoadFrom(roomSave[nodePath]);
          });

      // TODO Implement global loading

      ObjectOrder.OrganizeLayersInTree(GetTree());
    }

    public void SaveData(SaveFile saveFile = null)
    {
      this.DebugPrint("Getting Room Data...");

      saveFile ??= SaveManager.CurrentSaveFile;

      // Room-bound Persistent Node Saving
      var roomSaves = new Dictionary<string, ISerializable>();
      GetTree()
          .GetNodesInGroup(SaveManager.PersistGroup)
          .Cast<Node>()
          .ToList()
          .ForEach(node =>
          {
            if (!(node is IPersistant persistant))
              throw new Exception($"Node not persistant {node.Name}, {node.GetType()}");
            ISerializable nodeSave = persistant.Save();
            if (nodeSave is null) return;
            // this.DebugPrint($"\tSaved data from {node.Name}");
            roomSaves[GetPathTo(node)] = nodeSave;
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
              throw new Exception($"Node is not global persistant {node.Name}");

            ISerializable nodeSave = globalPersistant.GlobalSave();
            if (nodeSave is null) return;
            saveFile.GlobalData[globalPersistant.UniqueName] = nodeSave;
          });
    }
  }
}