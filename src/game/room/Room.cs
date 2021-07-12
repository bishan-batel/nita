using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;
using Godot.Collections;
using Parry2.editor;
using Parry2.game.actors.player;
using Parry2.managers.save;
using Parry2.managers.sound;
using Array = Godot.Collections.Array;

namespace Parry2.game.room
{
    public class Room : Node
    {
        [Export] public string RoomName = string.Empty;
        [Export] public NodePath Player;
        [Export] public NodePath DefaultGateway;

        public override void _Ready()
        {
            if (!RoomList.IsValidRoomName(RoomName))
                throw new Exception("Unauthorized name");

            LoadFromSave(SaveManager.CurrentSaveFile);

            var player = GetNode<PlayerShroom>(Player);

            GD.Print($"[{RoomName}] {GameplayScene.EnteredGate}");

            RoomGateway gateway = GetTree()
                .GetNodesInGroup(RoomGateway.GatewayGroup)
                .Cast<RoomGateway>()
                .FirstOrDefault(gate => gate.Name == GameplayScene.EnteredGate);

            if (gateway == null)
            {
                GD.Print($"[{RoomName}] unable to find {GameplayScene.EnteredGate}");
                gateway = GetNodeOrNull<RoomGateway>(DefaultGateway);
            }

            gateway?.EnteredCutscene(player);

            SaveData();
        }

        public override void _Process(float delta)
        {
#if DEBUG
            if (Input.IsActionJustPressed("debug_load_from_save"))
                GetTree().ReloadCurrentScene();
#endif
            //  LoadFromSave(SaveManager.CurrentSaveFile);

            Node currentScene = GetTree().CurrentScene;
            if (currentScene != this) return;
            GameplayScene.LoadRoom(this);
        }

        public void LoadFromSave(SaveFile saveFile = null)
        {
            saveFile ??= SaveManager.CurrentSaveFile;
            if (!saveFile.RoomData.ContainsKey(RoomName)) return;

            // Room-bound Persistent Node Loading
            var roomSave = saveFile.RoomData[RoomName];

            if (roomSave is null) return;
            GD.Print($"Loading {RoomName} from save file");

            roomSave
                .Keys
                .ToList()
                .ForEach(nodePath =>
                    GetNodeOrNull<IPersistant>(nodePath)
                        ?.LoadFrom(roomSave[nodePath])
                );

            // TODO Implement global loading

            ObjectOrder.OrganizeLayersInTree(GetTree());
        }

        public void SaveData(SaveFile saveFile = null)
        {
            saveFile ??= SaveManager.CurrentSaveFile;

            // Room-bound Persistent Node Saving
            var roomSaves = new System.Collections.Generic.Dictionary<string, ISerializable>();
            GetTree()
                .GetNodesInGroup(SaveManager.PersistGroup)
                .Cast<Node>()
                .ToList()
                .ForEach(node =>
                {
                    if (!(node is IPersistant persistant))
                        throw new Exception($"Node not persistant {node.Name}");
                    ISerializable nodeSave = persistant.Save();
                    if (nodeSave is null) return;
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