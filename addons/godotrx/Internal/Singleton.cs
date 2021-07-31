using System;
using Godot;
using Object = Godot.Object;

namespace Nita.addons.godotrx.Internal
{
  internal sealed class Singleton : Node
  {
    static readonly GDScript _gdInstanceScript = (GDScript) GD.Load("res://addons/godotrx/GodotRx.gd");
    static readonly Object _gdInstance = (Object) _gdInstanceScript.New();

    public static int RegisterInstanceTracker(InstanceTracker tracker, Object target)
    {
      var id = (int) _gdInstance.Call("inject_instance_tracker", target);
      _gdInstance.Connect("instance_tracker_freed", tracker, InstanceTracker.OnFreedMethod);
      return id;
    }

    #nullable disable
    public static Singleton Instance { get; private set; }
#nullable enable

    Singleton()
    {
      if (Instance != null) throw new InvalidOperationException();

      Instance = this;
      PauseMode = PauseModeEnum.Process;
    }
  }
}