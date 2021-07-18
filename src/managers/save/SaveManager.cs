using System.Linq;
using Godot;
using Godot.Collections;
using Parry2.game;
using Parry2.managers.game;
using Parry2.utils;

namespace Parry2.managers.save
{
    public class SaveManager : Node
    {
        public const string SaveDirPath = "user://saves/";
        public const string SaveFileType = ".dat";

        // Persist group is used for nodes bound to a room
        public const string PersistGroup = "Persist";

        // Global persist is for nodes that move between rooms such as Players 
        public const string GlobalPersistGroup = "GlobalPersist";

        public SaveManager()
        {
            Singleton = this;
        }

        public static SaveManager Singleton { private set; get; }
        public static string SavesDirAbsPath => OS.GetUserDataDir() + "/saves";

        public static SaveFile CurrentSaveFile { set; get; }

        public static Array<string> SaveSlots
        {
            get
            {
                Directory saveDir = OpenSaveDirectory();
                var slots = new Array<string>();


                // Grabs all files
                string nextPath;

                saveDir.ListDirBegin(true, true);
                while ((nextPath = saveDir.GetNext()) != "")
                {
                    if (!saveDir.FileExists(nextPath)) continue;
                    if (!nextPath.EndsWith(SaveFileType)) continue;
                    slots.Add(nextPath);
                }

                saveDir.ListDirEnd();

                return slots;
            }
        }

        public override void _Ready()
        {
            CurrentSaveFile ??= new SaveFile();
            CurrentSaveFile.Flush();

            if (OS.GetUserDataDir() == "user://")
                this.DebugPrintErr("Unable to find user:// directory");
        }

#if DEBUG
        public override void _Input(InputEvent @event)
        {
            if (Input.IsActionJustPressed("debug_save"))
                SaveDeferred();
        }
#endif

        public static void SaveDeferred() =>
            Singleton.CallDeferred(nameof(_save));

        public static void Save() =>
            Singleton._save();

        void _save()
        {
            this.DebugPrint($"Saving room to save '{CurrentSaveFile.Name}' in memory. . .");

            // TODO figure out wtf this TODO is saying

            // TODO Make sure if it is okay to save data outside of flush thread,
            // if it isn't then find way to copy instance of the current room before passing
            // into flush thread

            if (GameStateManager.CurrentState is not GameplayScene)
            {
                this.DebugPrintErr($"Unable to save state, wrong gamestate {GameStateManager.CurrentState}");
                return;
            }

            GameplayScene
                .CurrentRoom
                .SaveData(CurrentSaveFile);

            this.DebugPrint($"Flushing save '{CurrentSaveFile.Name}' to filesystem. . .");

            if (CurrentSaveFile.Flush()) this.DebugPrint("Flushed save successfully");
            else Singleton.DebugPrintErr($"Failed to flush save '{CurrentSaveFile.Name}'");
        }


        public static Directory OpenSaveDirectory()
        {
            var saveDir = new Directory();
            saveDir.MakeDir(SaveDirPath);

            saveDir.Open(SaveDirPath);
            return saveDir;
        }

        public static bool OpenSave(string path)
        {
            Singleton.DebugPrint($"Loading {path} save --");

            var dir = new Directory();
            if (!dir.DirExists(SavesDirAbsPath))
                dir.MakeDir(SavesDirAbsPath);

            SaveFile file = SaveFile.Open(path);
            if (file is null)
            {
                Singleton.DebugPrintErr($"\tFailed to open file {path}");
                return false;
            }

            CurrentSaveFile = file;
            Singleton.DebugPrint($"\tLoaded {path} save successfully");
            return true;
        }


        public static bool CreateNewSave(string name = null)
        {
            Singleton.DebugPrint($"Creating new save file {name}...");
            SaveFile file;

            // Name checking
            if (name == null)
            {
                file = new SaveFile();
            }
            else if (IsValidFileName(name))
            {
                file = new SaveFile(name);
            }
            else
            {
                Singleton.DebugPrintErr($"\tInvalid filename {name}");
                return false;
            }

            // Flushes to filesystem and checks result
            bool result = file.Flush();
            if (result)
                Singleton.DebugPrint($"\tCreated new save file {name}");
            else
                Singleton.DebugPrintErr($"\tFailed to create file {name}");

            return result;
        }

        public static string FormatAbsPath(string name) => SavesDirAbsPath.PlusFile(name + SaveFileType);

        public static string GetSaveFileName(string path)
        {
            string name = path.Split("/").Last();
            return name.Remove(name.Length() - SaveFileType.Length());
        }

        public static bool IsValidFileName(string name)
        {
            // TODO write this lol or else game crashes
            return true;
        }
    }
}