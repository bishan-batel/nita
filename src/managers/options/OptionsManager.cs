using System;
using Godot;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Nita.debug;
using File = System.IO.File;

namespace Nita.managers.options
{
  public class OptionsManager : Node
  {
    public static string OptionsPath => OS.GetUserDataDir().PlusFile("options.dat");
    public static OptionsManager Singleton { private set; get; }
    public static OptionsData Options => _options ?? LoadOptions();
    static OptionsData _options;

    public override void _Ready()
    {
      Singleton = this;
      LoadOptions();

      // Add Commands
      AddCommands();
    }


    public static OptionsData LoadOptions()
    {
      if (!File.Exists(OptionsPath))
      {
        _options = new OptionsData();
        SaveOptions();
        return _options;
      }

      using FileStream fs = File.OpenRead(OptionsPath);
      var bf = new BinaryFormatter();
      _options = (OptionsData)bf.Deserialize(fs);
      return _options;
    }

    public static void SaveOptions()
    {
      using FileStream fs = File.Create(OptionsPath);
      var bf = new BinaryFormatter();
      bf.Serialize(fs, _options);
    }

    void AddCommands()
    {
      this.AddCommand("option.controller_mode",
        "Changes Controller mode",
        nameof(ChangeControllerMode),
        ("mode (1 or 0)", Variant.Type.Int)
      );
    }

    void ChangeControllerMode(int mode)
    {
      var controllerMode = (OptionsData.ControllerModeE)mode;
      Options.ControllerMode.SetTo(controllerMode);
      GConsole.WriteLine($"Changed controller mode to ${controllerMode.ToString()}");
    }
  }
}