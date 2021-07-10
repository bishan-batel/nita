using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Parry2.editor;
using Parry2.game.room;
using Parry2.managers.save;
using Parry2.managers.sound;
using Array = Godot.Collections.Array;

namespace Parry2.game.rooms
{
    public class Room : Node
    {
        [Export] public string RoomName = string.Empty;
        [Export] public NodePath Player;

        [Export(PropertyHint.ResourceType, "MusicSettings")]
        public MusicSettings MusicSettings;

        [Export] public Array<NodePath> Gateways { set; get; }


        public override void _Ready()
        {
            if (!RoomList.IsValidRoomName(RoomName))
                throw new Exception("Unauthorized name");

            LoadFromSave(SaveManager.CurrentSaveFile);
            SaveData();

            if (!(MusicSettings is null))
                SoundManager.Play(MusicSettings);

            // TODO make gateways work
            // if (Gateways is null) return;
            // if (EnteredGate >= Gateways.Count) return;
            //
            // var gateway = GetNodeOrNull<ChapterGateway>(Gateways[EnteredGate]);
            // if (gateway is null) return;
            //
            // GetNode<PlayerShroom>(Player).GlobalPosition = gateway.GlobalPosition;
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