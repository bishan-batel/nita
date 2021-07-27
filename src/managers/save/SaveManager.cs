using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Godot.Collections;
using Parry2.game;
using Parry2.managers.game;
using Parry2.utils;
using Path = System.IO.Path;

namespace Parry2.managers.save
{
  public class SaveManager : Node
  {
    public const string SaveDirPath = "user://saves/";
    public const string SaveFileType = ".dat";

    /// <summary>
    /// Persist group used for nodes with state to be saved to it's designated room
    /// </summary>
    public const string PersistGroup = "Persist";

    /// <summary>
    /// Global persist used for special cases like players whose state is not bound to a single room
    /// </summary>
    public const string GlobalPersistGroup = "GlobalPersist";

    public SaveManager() =>
        Singleton = this;

    public static SaveManager Singleton { private set; get; }

    /// <summary>
    /// Retrieves absolute path for the save directory
    /// </summary>
    public static string SavesDirAbsPath => OS.GetUserDataDir() + "/saves";

    /// <summary>
    /// Current loaded save file (use this over any other save file that is not loaded unless absolutely required)
    /// </summary>
    public static SaveFile CurrentSaveFile { set; get; }

    /// <summary>
    /// Returns list of saves in save directory
    /// </summary>
    public static IEnumerable<string> SaveSlots
    {
      get
      {
        Godot.Directory saveDir = OpenSaveDirectory();
        var slots = new Array<string>();


        // Grabs all files
        string nextPath;

        saveDir.ListDirBegin(true, true);

        while ((nextPath = saveDir.GetNext()) != string.Empty)
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

    // TODO remove for final production
#if DEBUG
    public override void _Input(InputEvent @event)
    {
      if (Input.IsActionJustPressed("debug_save"))
        Save();
    }
#endif

    /// <summary>
    /// Saves game on idle frames
    /// </summary>
    /// <param name="file">Save file to save the game to (will default to the currently loaded save file in SaveManager)</param>
    public static void SaveDeferred(SaveFile file = null) =>
        Singleton.CallDeferred(nameof(_save), file);

    /// <summary>
    /// Saves game (Warning: will not wait for idles frames meaning depending on how many objects there are to save it may cause a lag spike)
    /// </summary>
    /// <param name="file">Save file to save the game to (will default to the currently loaded save file in SaveManager)</param>
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
        throw new WrongGamestateException(typeof(GameplayScene));

      if (GameplayScene.CurrentRoom is null)
        throw new NullReferenceException("Unable to save state, no room loaded");

      // Saves
      GameplayScene.CurrentRoom.SaveData(file);

      if (CurrentSaveFile.Flush()) this.DebugPrint("|\t> Flushed save successfully");
      else throw new IOException($"Failed to flush save '{file.Name}'");
    }

    /// <summary>
    /// Returns a Godot.Directory opened into the game's save directory.
    /// This will create the save directory if it does not exist.
    /// </summary>
    /// <returns>Godot.Directory opened to game's save directory</returns>
    public static Godot.Directory OpenSaveDirectory()
    {
      var saveDir = new Godot.Directory();
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
      CurrentSaveFile = SaveFile.Open(path);
      return CurrentSaveFile is not null;
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
    public static string FormatAbsPath(string name) => SavesDirAbsPath.PlusFile(name + SaveFileType);

    /// <summary>
    /// Checks if save file name is valid to save
    /// </summary>
    /// <param name="name">Save file name (omit parent directory)</param>
    /// <returns>Validity of filename</returns>
    public static bool IsValidFileName(string name)
    {
      return
          !string.IsNullOrEmpty(name) &&
          name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
          !IsInvalidWinName(name);
    }

    public static readonly string[] InvalidWindowFilenames =
    {
      "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
    };

    /// <summary>
    /// Used to check if a files name (without extensions) matches the window's list of
    /// unusable filenames
    /// </summary>
    /// <param name="name">Filename to check (omit any extensions)</param>
    /// <returns>Invalidity of file name</returns>
    public static bool IsInvalidWinName(string name) => InvalidWindowFilenames
        .ToList()
        .Any(invalid => string.Equals(name, invalid, StringComparison.CurrentCultureIgnoreCase));
  }
}