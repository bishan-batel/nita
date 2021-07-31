using System;
using System.Collections.Generic;

namespace Nita.addons.godotrx.Internal
{
  internal static class ListExtensions
  {
    internal static void SafeForEach<T>(this List<T> list, Action<T> action)
    {
      foreach (T e in list.ToArray())
        action(e);
    }
  }
}