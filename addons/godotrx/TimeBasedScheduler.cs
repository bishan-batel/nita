using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Godot;
using Nita.addons.godotrx.Internal;

namespace Nita.addons.godotrx
{
  public sealed class TimeBasedScheduler : IScheduler
  {
    public static readonly TimeBasedScheduler Process = new(true);
    public static readonly TimeBasedScheduler Inherit = new(false);

    readonly bool _pauseModeProcess;

    TimeBasedScheduler(bool pauseModeProcess) =>
        _pauseModeProcess = pauseModeProcess;

    public DateTimeOffset Now => DateTimeOffset.FromUnixTimeMilliseconds((long) OS.GetSystemTimeMsecs());

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action) =>
        DefaultScheduler.Instance.Schedule(state, action);

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
      if (dueTime.TotalSeconds <= 0) return Schedule(state, action);

      var disposable = new BooleanDisposable();
      DelayAction(dueTime, state, disposable, action);
      return disposable;
    }

    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) =>
        Schedule(state, dueTime - Now, action);

    async void DelayAction<TState>(TimeSpan dueTime, TState state, BooleanDisposable disposable, Func<IScheduler, TState, IDisposable> action)
    {
      await Singleton.Instance.WaitFor(dueTime, _pauseModeProcess);
      if (!disposable.IsDisposed) action(this, state);
    }
  }
}