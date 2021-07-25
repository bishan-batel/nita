using System;
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

    public SaveManager() =>
        Singleton = this;

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
        Save();
    }
#endif

    public static void SaveDeferred(SaveFile file = null) =>
        Singleton.CallDeferred(nameof(_save), file);

    public static void Save(SaveFile file = null) => Singleton._save(file);

    void _save(SaveFile file)
    {
      file ??= CurrentSaveFile;

      this.DebugPrint($"Saving to file [{file.Name}]");

      // TODO figure out wtf this TODO is saying

      // TODO Make sure if it is okay to save data outside of flush thread,
      // if it isn't then find way to copy instance of the current room before passing
      // into flush thread

      // Asserts that game is in gameplay mode
      if (GameStateManager.CurrentState is not GameplayScene)
        throw new Exception($"Unable to save state, wrong gamestate [{GameStateManager.CurrentState}]");

      if (GameplayScene.CurrentRoom is null)
        throw new Exception("|\t> Unable to save state, no room loaded");

      // Saves
      GameplayScene.CurrentRoom.SaveData(file);

      if (CurrentSaveFile.Flush()) this.DebugPrint("|\t> Flushed save successfully");
      else throw new Exception($"Failed to flush save '{file.Name}'");
    }


    /// <summary>
    /// Returns a Godot.Directory opened into the game's save directory.
    /// This will create the save directory if it does not exist.
    /// </summary>
    /// <returns>Godot.Directory opened to game's save directory</returns>
    public static Directory OpenSaveDirectory()
    {
      var saveDir = new Directory();
      saveDir.MakeDir(SaveDirPath);

      saveDir.Open(SaveDirPath);
      return saveDir;
    }

    /// <summary>
    /// Opens save in specified filepath 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool OpenSave(string path)
    {
      Singleton.DebugPrint($"Loading {path}");

      // Assert that save directory exists
      OpenSaveDirectory();

      return SaveFile.Open(path) is not null;
    }


    /// <summary>
    /// Creates a new empty save file of specified name
    /// </summary>
    /// <param name="name">Name of new save file</param>
    /// <returns>Returns true if save was created succesfully</returns>
    public static bool CreateNewSave(string name = null)
    {
      Singleton.DebugPrint($"Creating new save file {name}...");
      SaveFile file;

      // Name checking
      if (name is null)
        file = new SaveFile();

      else if (IsValidFileName(name))
        file = new SaveFile(name);

      else
      {
        Singleton.DebugPrintErr($"|\t>Invalid filename {name}");
        return false;
      }

      // Flushes to filesystem and checks result
      return file.Flush();
    }

    /// <summary>
    /// Used to get the full file path to a save file of specified name
    /// </summary>
    /// <param name="name">File name</param>
    /// <returns></returns>
    public static string FormatAbsPath(string name) =>
        SavesDirAbsPath.PlusFile(name + SaveFileType);

    public static string GetSaveFileName(string path)
    {
      string name = path.Split("/").Last();
      return name.Remove(name.Length() - SaveFileType.Length());
    }

    public static bool IsValidFileName(string name) =>
        // TODO write this lol or else game crashes
        true;
  }
}