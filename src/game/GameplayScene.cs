using System.Linq;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using GodotRx;
using Parry2.game.room;
using Parry2.managers.game;
using Parry2.managers.save;
using Parry2.utils;
using System;
using Parry2.debug;

namespace Parry2.game
{
    public class GameplayScene : GameState
    {
        public static PackedScene PackedScene =>
            ResourceLoader.Load<PackedScene>("res://src/game/GameplayScene.tscn");

        public static GameplayScene Singleton;
        public static string EnteredGate;
        [Export] public string DefaultChapterName = "test_room";

        [Node("ChapterViewContainer/ChapterContainer")]
        public readonly Node ChapterContainer = null;

        [Node("UI/DialogueManager")] public readonly GameDialogueManager DialogueManager = null;

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


        public static void LoadRoom(string roomName, string gateway = null)
        {
            // Singleton.DebugPrint($"Loading into {roomName} through gateway {gateway ?? "[default]"}");

            CurrentRoom
                .OnTreeExiting()
                .Subscribe(_ =>
                {
                    EnteredGate = gateway;

                    CurrentRoom = RoomList.GetChapterScene(roomName).Instance<Room>();
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

        public static void LoadFromSave(SceneTree tree, SaveFile file)
        {
            tree.ChangeSceneTo(PackedScene);
            CurrentRoom = (Room) file.CurrentRoom.Instance();
        }

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