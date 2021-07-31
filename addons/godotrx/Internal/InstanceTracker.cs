using System;
using System.Collections.Generic;
using Object = Godot.Object;

namespace Nita.addons.godotrx.Internal
{
  internal sealed class InstanceTracker : Object
  {
    public static readonly string OnFreedMethod = nameof(OnTrackerFreed);

    static readonly Dictionary<ulong, InstanceTracker> store = new();

    readonly int _id;

    InstanceTracker() { }

    InstanceTracker(Object target) =>
        _id = Singleton.RegisterInstanceTracker(this, target);

    public event Action? Freed;

    public static InstanceTracker Of(Object target)
    {
      ulong instId = target.GetInstanceId();

      if (!store.TryGetValue(instId, out InstanceTracker tracker))
      {
        tracker = new InstanceTracker(target);
        store[instId] = tracker;
        tracker.Freed += () => store.Remove(instId);
      }

      return tracker;
    }

    public void OnTrackerFreed(int id)
    {
      if (_id == id)
      {
        Freed?.Invoke();
        this.DeferredFree();
      }
    }

    protected override void Dispose(bool disposing)
    {
      // GD.Print("InstanceTracker disposed");
      base.Dispose(disposing);
    }
  }
}