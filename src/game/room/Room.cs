using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;
using Parry2.editor;
using Parry2.game.world.objects.checkpoint;
using Parry2.managers.save;
using Parry2.utils;
using PlayerShroom = Parry2.game.world.actors.player.PlayerShroom;

namespace Parry2.game.room
{
    public class Room : Node
    {
        [Export] public NodePath DefaultGateway;
        [Export] public NodePath Player;
        [Export] public string RoomName = string.Empty;


        public static CheckpointManagerNode CheckpointManager => GameplayScene
            .Singleton
            .GetNode<CheckpointManagerNode>("CheckpointManager");

        public override void _Ready()
        {
            if (GetTree().CurrentScene == this)
            {
                GameplayScene.LoadRoom(this);
                return;
            }

            if (!RoomList.IsValidRoomName(RoomName))
                throw new Exception("Unauthorized name");

            LoadFromSave(SaveManager.CurrentSaveFile);

            var player = GetNode<PlayerShroom>(Player);

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
                this.DebugPrint($"Using checkpoint @ {location}");
                GetNode<PlayerShroom>(Player).GlobalPosition = location;
            }
            else
            {
                var @default = GetNodeOrNull<RoomGateway>(DefaultGateway);
                @default?.EnteredCutscene(player);
                this.DebugPrint($"Entering room through default gateway [{@default?.Name ?? "null"}]");
            }


            // if (Checkpoint.ClaimedPath == null)


            SaveData();
        }

        public void OnDeath()
        {
            GameplayScene.LoadRoom(RoomName);
        }

        public override void _Process(float delta)
        {
#if DEBUG
            if (Input.IsActionJustPressed("debug_load_from_save"))
                GetTree().ReloadCurrentScene();
#endif
            //  LoadFromSave(SaveManager.CurrentSaveFile);
        }

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