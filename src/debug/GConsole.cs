using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Nita.addons.godotrx;
using Nita.utils;
using Object = Godot.Object;

namespace Nita.debug
{
  public static class GConsole
  {
    public static Node Singleton => Global.Singleton?.GetNode("/root/Console");

    // Signals
    public static IObservable<bool> OnToggled() => Singleton.ObserveSignal<bool>("toggled");
    public static IObservable<(string name, Reference reference, string targetName)> OnCommandAdded() => Singleton.ObserveSignal<string, Reference, string>("command_added");
    public static IObservable<string> OnCommandRemoved() => Singleton.ObserveSignal<string>("command_removed");
    public static IObservable<string> OnCommandNotFound() => Singleton.ObserveSignal<string>("command_not_found");
    public static IObservable<Object> OnCommandExecuted() => Singleton.ObserveSignal<Object>("command_executed");


    public static void WriteLine(string msg) => Singleton.Call("write_line", msg);

    public static void Write(string msg) => Singleton.Call("write", msg);

    public static void Clear() => Singleton.Call("clear");

    public static void ToggleConsole() => Singleton.Call("toggle_console");

    public static void AddCommand(
      this Node instance,
      string name,
      string description,
      string funcName,
      params (string name, Variant.Type type)[] args)
    {
      new Command(instance, funcName, name, description, args)
          .Register();
    }

    class Command : Object
    {
      readonly List<(string name, Variant.Type type)> _arguments;
      readonly Object _instance;
      readonly string _name, _description, _funcName;

      public Command() : this(null, null, null, null)
      {
      }

      public Command(
        Node instance,
        string funcName,
        string name,
        string description,
        params (string, Variant.Type)[] args
      )
      {
        _instance = instance;
        _funcName = funcName;
        _name = name;
        _arguments = args.ToList();
        _description = description ?? string.Empty;

        instance
            .OnTreeExiting()
            .Subscribe(_ =>
            {
              Singleton.DebugPrint($"Removed Command {_name}");
              Singleton.Call("remove_command", _name);
            })
            .DisposeWith(this);
      }

      public void Register()
      {
        var cmd = (Object)Singleton.Call("add_command", _name, _instance, _funcName);
        cmd = (Object)cmd.Call("set_description", _description);

        _arguments
            .Aggregate(cmd, (chain, arg) =>
                (Object)chain.Call("add_argument", arg.name, arg.type))
            .Call("register");
      }
    }
  }
}