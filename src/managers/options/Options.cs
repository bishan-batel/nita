using System;
using System.Linq;
using System.Reactive.Linq;
using Godot;
using Godot.Collections;
using Nita.debug;

namespace Nita.managers.options
{
  public enum Option
  {
    UseController,
    Volume
  }

  public class Options : Node
  {
    public static Options Singleton;
    Dictionary<string, object> _values;

    public Options()
    {
      Singleton = this;
    }

    public override void _Ready()
    {
      _values = new Dictionary<string, object>();
      _setupCommands();
      _setupDefaultOptions();
    }

    void _setupDefaultOptions()
    {
      PutIfEmpty(Option.UseController, false);
    }

    void _setupCommands()
    {
      this.AddCommand(
        "options_get",
        "Retrieves all available options to customize",
        nameof(_cmdGetOptions),
        ("option", Variant.Type.String)
      );

      this.AddCommand(
        "option_set",
        "Sets an option to value",
        nameof(_cmdSetOption),
        ("option", Variant.Type.String),
        ("value", Variant.Type.String)
      );
    }

    void _cmdSetOption(string optionStr, string val)
    {
      if (!Enum.TryParse(optionStr, out Option option))
      {
        GConsole.WriteLine($"[red]Unable to find option {optionStr}");
        return;
      }

      switch (option)
      {
        case Option.UseController:
          PutTryParseToBool(Option.UseController, val);
          return;
        case Option.Volume:
          PutTryParseToInt(Option.Volume, val);
          return;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    void _cmdGetOptions()
    {
      typeof(Option)
          .GetEnumValues()
          .Cast<Option>()
          .Select(opt => opt.ToString())
          .ToList()
          .ForEach(GConsole.WriteLine);
    }


    public static void PutIfEmpty(Option opt, object val)
    {
      if (Singleton._values.ContainsKey(opt.ToString())) return;
      Put(opt, val);
    }

    public static void PutTryParseToInt(Option opt, string valStr)
    {
      if (int.TryParse(valStr, out int val))
        Put(opt, val);
    }

    public static void PutTryParseToBool(Option opt, string valStr)
    {
      if (bool.TryParse(valStr, out bool val))
        Put(opt, val);
    }

    public static void Put(Option opt, object val)
    {
      var key = opt.ToString();
      Singleton._values[key] = val;
      GConsole.WriteLine($"Option {key} set to {val}");
    }

    public static T Get<T>(Option opt) => (T)Singleton._values[opt.ToString()];

    public static T GetOrDefault<T>(Option opt, T fallback)
    {
      var key = opt.ToString();
      if (!Singleton._values.ContainsKey(key))
      {
        return fallback;
      }

      object val = Singleton._values[key];
      if (val is T casted) return casted;

      return fallback;
    }
  }
}