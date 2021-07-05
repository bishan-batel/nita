#define DEBUG

using System;
using Godot;

namespace Parry2
{
    public class Global : CanvasLayer
    {
        public Global()
        {
            Singleton = Singleton == null
                ? this
                : throw
                    new Exception("Duplicate " + nameof(Global) + "found");
        }

        public static Global Singleton { private set; get; }

        public override void _Process(float delta)
        {
        }

        public override void _Input(InputEvent @event)
        {
            if (!(@event is InputEventKey eventKey)) return;

#if DEBUG
            if (eventKey.IsActionPressed("debug_restart"))
                GetTree().ReloadCurrentScene();
#endif

            if (eventKey.IsActionPressed("fullscreen_toggle"))
                OS.WindowFullscreen = !OS.WindowFullscreen;
        }
    }
}