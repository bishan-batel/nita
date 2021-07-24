#define DEBUG

using System;
using System.Reactive.Linq;
using Godot;
using GodotRx;
using Parry2.debug;

namespace Parry2
{
    public class Global : Node
    {
        public Global()
        {
            Singleton = Singleton == null
                ? this
                : throw
                    new Exception("Duplicate " + nameof(Global) + "found");
        }

        public static Global Singleton { private set; get; }


        public override void _Ready()
        {
            PauseMode = PauseModeEnum.Process;
            this.AddCommand("exit", "Exits out of console", nameof(Exit));

            GConsole
                .OnToggled()
                .Subscribe(shown => GetTree().Paused = shown)
                .DisposeWith(this);
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