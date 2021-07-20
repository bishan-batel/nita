using Godot;
using Parry2.game.room;
using Parry2.managers.game;
using Parry2.managers.save;

namespace Parry2.game
{
    public class GameplayScene : GameState
    {
        public static GameplayScene Singleton;
        public static string EnteredGate;
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

            CurrentRoom ??= SaveManager
                .CurrentSaveFile
                ?.CurrentRoom
                ?.Instance() as Room;

            Node chapterContainer = GetNode("ChapterContainer");

            // Clears Container
            foreach (Node child in chapterContainer.GetChildren())
                child.Free();

            chapterContainer.AddChild(CurrentRoom);
        }


        public static void LoadRoom(string roomName, string gateway = null)
        {
            // Singleton.DebugPrint($"Loading into {roomName} through gateway {gateway ?? "[default]"}");
            EnteredGate = gateway;

            Global
                .Singleton
                .GetTree()
                .ChangeSceneTo(RoomList.GetChapterScene(roomName));
        }

        public static void LoadRoom(Room room)
        {
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
    }
}