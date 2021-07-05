using Godot;
using Parry2.game.rooms;
using Parry2.managers.game;
using Parry2.managers.save;

namespace Parry2.game
{
    public class GameplayScene : GameState
    {
        [Export] public PackedScene DefaultChapter;

        //  I would use default parameters to have just one constructor, but godot
        // seems to just crash when trying to instance it
        public GameplayScene() : this(null)
        {
        }

        public GameplayScene(Room room) : base(nameof(GameplayScene))
        {
            CurrentRoom = room;
        }

        public static PackedScene PackedScene
            => ResourceLoader.Load<PackedScene>("res://src/game/GameplayScene.tscn");

        public static Room CurrentRoom { set; get; }

        public override void _Ready()
        {
            base._Ready();

            CurrentRoom ??= SaveManager.CurrentSaveFile?.CurrentChapter?.Instance() as Room;
            // CurrentRoom ??= DefaultChapter.Instance<Room>();

            Node chapterContainer = GetNode("ChapterContainer");

            // Clears Container
            foreach (Node child in chapterContainer.GetChildren())
                child.Free();

            chapterContainer.AddChild(CurrentRoom);
        }

        public static void LoadFromChapter(Room room)
        {
            room.GetTree().ChangeSceneTo(PackedScene);
            CurrentRoom = (Room) room.Duplicate();
            room.Free();
        }

        public static void LoadFromSave(SceneTree tree, SaveFile file)
        {
            tree.ChangeSceneTo(PackedScene);
            CurrentRoom = file.CurrentChapter.Instance<Room>();
        }
    }
}