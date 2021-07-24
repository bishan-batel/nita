using System;
using System.Collections.Generic;
using GodotRx.Internal;
using Object = Godot.Object;

namespace GodotRx
{
  public static class IDisposableExtensions
  {
    static readonly Dictionary<ulong, HashSet<IDisposable>> objectDisposables = new();

    public static void DisposeWith(this IDisposable disposable, Object obj)
    {
      ulong instId = obj.GetInstanceId();

      if (!objectDisposables.ContainsKey(instId))
      {
        objectDisposables[instId] = new HashSet<IDisposable>();

        InstanceTracker.Of(obj).Freed += () =>
        {
          foreach (IDisposable disposable in objectDisposables[instId])
              // GD.Print($"disposed with {instId}");
            disposable.Dispose();

          objectDisposables.Remove(instId);
        };
      }

      objectDisposables[instId].Add(disposable);
    }
  }
}