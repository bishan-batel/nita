#define DEBUG

using System;
using Godot;
using GodotRx;
using Parry2.debug;

namespace Parry2
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
      this.AddCommand("exit", "Exits out of console", nameof(Exit));
    }

    public void Exit()
    {
      GConsole.Clear();
      GConsole.ToggleConsole();
    }

    public override void _Input(InputEvent @event)
    {
      if (@event is not InputEventKey eventKey) return;

#if DEBUG
      if (eventKey.IsActionPressed("debug_restart"))
        GetTree().ReloadCurrentScene();
#endif

      if (eventKey.IsActionPressed("fullscreen_toggle"))
        OS.WindowFullscreen = !OS.WindowFullscreen;
    }
  }
}