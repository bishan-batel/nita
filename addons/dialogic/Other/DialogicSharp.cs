using Godot;
using GC = Godot.Collections;

namespace Nita.addons.dialogic.Other
{
  public static class DialogicSharp
  {
    const string DEFAULT_DIALOG_RESOURCE = "res://addons/dialogic/Dialog.tscn";
    static readonly Script _dialogic = GD.Load<Script>("res://addons/dialogic/Other/DialogicClass.gd");

    public static string CurrentTimeline
    {
      get => (string) _dialogic.Call("get_current_timeline");
      set => _dialogic.Call("set_current_timeline", value);
    }

    public static GC.Dictionary Definitions => (GC.Dictionary) _dialogic.Call("get_definitions");

    public static GC.Dictionary DefaultDefinitions => (GC.Dictionary) _dialogic.Call("get_default_definitions");

    public static bool Autosave
    {
      get => (bool) _dialogic.Call("get_autosave");
      set => _dialogic.Call("set_autosave", value);
    }

    public static Node Start(string timeline, bool resetSaves = true, bool debugMode = false) =>
        Start<Node>(timeline, DEFAULT_DIALOG_RESOURCE, resetSaves, debugMode);

    public static T Start<T>(string timeline, string dialogScenePath, bool resetSaves = true, bool debugMode = false) where T : class =>
        (T) _dialogic.Call("start", timeline, resetSaves, dialogScenePath, debugMode);

    public static Node StartFromSave(string timeline, bool debugMode = false) =>
        StartFromSave<Node>(timeline, DEFAULT_DIALOG_RESOURCE, debugMode);

    public static T StartFromSave<T>(string timeline, string dialogScenePath, bool debugMode = false) where T : class =>
        (T) _dialogic.Call("start", timeline, dialogScenePath, debugMode);

    public static string GetVariable(string name) =>
        (string) _dialogic.Call("get_variable", name);

    public static void SetVariable(string name, string value)
    {
      _dialogic.Call("set_variable", name, value);
    }

    public static GC.Dictionary GetGlossary(string name) =>
        (GC.Dictionary) _dialogic.Call("get_glossary", name);

    public static void SetGlossary(string name, string title, string text, string extra)
    {
      _dialogic.Call("set_glossary", name, title, text, extra);
    }

    public static void ResetSaves()
    {
      _dialogic.Call("reset_saves");
    }

    public static Error SaveDefinitions() =>
        (Error) _dialogic.Call("save_definitions");

    public static GC.Dictionary Export() =>
        (GC.Dictionary) _dialogic.Call("export");

    public static void Import(GC.Dictionary data)
    {
      _dialogic.Call("import", data);
    }
  }
}