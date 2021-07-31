#define DEBUG

using System;
using System.Reactive.Linq;
using Godot;
using Nita.addons.godotrx;
using Nita.debug;

namespace Nita
{
  public class Global : Node
  {
    // Window constants
    public const float AspectRatio = 16f / 9f;
    public static readonly (int width, int height) WindowSize = (300, 268);

    // Upscale window constants used for rendering UI / text
    public static readonly (int width, int height) UpscaledWindowSize = (1600, 900);
    public static readonly float WinUpscaleFactor = WindowSize.width / (float) UpscaledWindowSize.width;

    public Global() => Singleton = Singleton is null
        ? this
        : throw
            new Exception("Duplicate " + nameof(Global) + "found");

    public static Global Singleton { private set; get; }


    public override void _Ready()
    {
      PauseMode = PauseModeEnum.Process;

      AddKeybindings();

      // Commands
      AddDefaultCommands();

      // Pauses game when console toggled
      GConsole
          .OnToggled()
          .Subscribe(shown => GetTree().Paused = shown)
          .DisposeWith(this);
    }

    void AddDefaultCommands()
    {
      this.AddCommand("exit", "Exits out of console", nameof(CmdExit));
      this.AddCommand(
        "timescale",
        "Speeds up or slows down game by a factor",
        nameof(CmdTimescale),
        ("factor", Variant.Type.Real)
      );
    }

    public void CmdTimescale(float factor)
    {
      GConsole.ToggleConsole();
      GConsole.Write($"Changed time scale to {factor}");
      Engine.TimeScale = factor;
    }

    public void CmdExit()
    {
      GConsole.Clear();
      GConsole.ToggleConsole();
    }

    public void AddKeybindings()
    {
      var actionEventObservable = this
          .OnInput()
          .Where(@event => @event is InputEventAction)
          .Cast<InputEventAction>();

      // Fullscreen toggle
      actionEventObservable
          .Where(@event => @event.IsActionPressed("fullscreen_toggle"))
          .Subscribe(_ => OS.WindowFullscreen = !OS.WindowFullscreen);

#if DEBUG
      // Debug restart
      actionEventObservable
          .Where(@event => @event.IsActionPressed("debug_restart"))
          .Subscribe(_ => GetTree().ReloadCurrentScene());
#endif
    }
  }

  public class GroupInterfaceException : Exception
  {
    public readonly Node Node;
    public readonly Type Expected;

    public override string Message => $"Group [{Expected.Name}] expected in node [{Node.Name}]";

    /// <summary>
    /// Exception used when you are trying to sync all instances of a godot group to classes with an interface 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="expectedGroup"></param>
    public GroupInterfaceException(Node node, Type expectedGroup)
    {
      Node = node;
      Expected = expectedGroup;
    }
  }
}