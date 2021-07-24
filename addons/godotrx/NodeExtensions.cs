using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Godot;
using GodotRx.Internal;

namespace GodotRx
{
  public static class NodeExtensions
  {
        #region Lifecycle

    public static IObservable<float> OnProcess(this Node node)
      => node.GetNodeTracker().OnProcess;

    public static IObservable<float> OnPhysicsProcess(this Node node)
      => node.GetNodeTracker().OnPhysicsProcess;

    public static IObservable<InputEvent> OnInput(this Node node)
      => node.GetNodeTracker().OnInput;

    public static IObservable<InputEvent> OnUnhandledInput(this Node node)
      => node.GetNodeTracker().OnUnhandledInput;

    public static IObservable<InputEventKey> OnUnhandledKeyInput(this Node node)
      => node.GetNodeTracker().OnUnhandledKeyInput;

    static NodeTracker GetNodeTracker(this Node node)
    {
      var tracker = node.GetNodeOrNull<NodeTracker>(NodeTracker.DefaultName);

      if (tracker == null)
      {
        tracker = new NodeTracker
        {
            Name = NodeTracker.DefaultName
        };
        node.AddChild(tracker);
      }

      return tracker;
    }

        #endregion

        #region Input

    public static IObservable<InputEventMouseButton> OnMouseDown(this Node node,
      ButtonList button = ButtonList.Left)
      => node.OnMouseButtonEvent(false, button, true);

    public static IObservable<InputEventMouseButton> OnMouseUp(this Node node, ButtonList button = ButtonList.Left)
      => node.OnMouseButtonEvent(false, button, false);

    public static IObservable<InputEventMouseButton> OnUnhandledMouseDown(this Node node,
      ButtonList button = ButtonList.Left)
      => node.OnMouseButtonEvent(true, button, true);

    public static IObservable<InputEventMouseButton> OnUnhandledMouseUp(this Node node,
      ButtonList button = ButtonList.Left)
      => node.OnMouseButtonEvent(true, button, false);

    static IObservable<InputEventMouseButton> OnMouseButtonEvent(this Node node, bool unhandled,
      ButtonList button, bool pressed)
    {
      return (unhandled ? node.OnUnhandledInput() : node.OnInput())
          .OfType<InputEvent, InputEventMouseButton>()
          .Where(ev => ev.ButtonIndex == (int) button && ev.Pressed == pressed);
    }

    public static IObservable<InputEventKey> OnKeyPressed(this Node node, KeyList key)
      => node.OnKeyEvent(false, key, true, null);

    public static IObservable<InputEventKey> OnKeyReleased(this Node node, KeyList key)
      => node.OnKeyEvent(false, key, false, null);

    public static IObservable<InputEventKey> OnKeyJustPressed(this Node node, KeyList key)
      => node.OnKeyEvent(false, key, true, false);

    public static IObservable<InputEventKey> OnUnhandledKeyPressed(this Node node, KeyList key)
      => node.OnKeyEvent(true, key, true, null);

    public static IObservable<InputEventKey> OnUnhandledKeyReleased(this Node node, KeyList key)
      => node.OnKeyEvent(true, key, false, null);

    public static IObservable<InputEventKey> OnUnhandledKeyJustPressed(this Node node, KeyList key)
      => node.OnKeyEvent(true, key, true, false);

    static IObservable<InputEventKey> OnKeyEvent(this Node node, bool unhandled, KeyList key, bool pressed,
      bool? echo)
    {
      return (unhandled ? node.OnUnhandledInput() : node.OnInput())
          .OfType<InputEvent, InputEventKey>()
          .Where(ev => ev.Scancode == (uint) key && ev.Pressed == pressed && (echo == null || ev.Echo == echo));
    }

    public static IObservable<Unit> OnActionPressed(this Node node, string action)
    {
      return node.OnProcess()
          .Where(_ => Input.IsActionPressed(action))
          .Select(_ => new Unit());
    }

    public static IObservable<Unit> OnActionJustPressed(this Node node, string action)
    {
      return node.OnProcess()
          .Where(_ => Input.IsActionJustPressed(action))
          .Select(_ => new Unit());
    }

    public static IObservable<Unit> OnActionJustReleased(this Node node, string action)
    {
      return node.OnProcess()
          .Where(_ => Input.IsActionJustReleased(action))
          .Select(_ => new Unit());
    }

        #endregion

        #region Frames

    public static IObservable<float> OnIdleFrame(this Node node)
    {
      return node.GetTree().OnIdleFrame()
          .Select(_ => node.GetProcessDeltaTime());
    }

    public static IObservable<float> OnNextIdleFrame(this Node node) =>
        node.OnIdleFrame().Take(1);

    public static Task<float> WaitNextIdleFrame(this Node node) =>
        node.OnNextIdleFrame().ToTask();

    public static IObservable<float> OnPhysicsFrame(this Node node)
    {
      return node.GetTree().OnPhysicsFrame()
          .Select(_ => node.GetPhysicsProcessDeltaTime());
    }

    public static IObservable<float> OnNextPhysicsFrame(this Node node) =>
        node.OnPhysicsFrame().Take(1);

    public static Task<float> WaitNextPhysicsFrame(this Node node) =>
        node.OnNextPhysicsFrame().ToTask();

        #endregion

        #region Time

    public static Task WaitFor(this Node node, TimeSpan duration, bool pauseModeProcess = false) =>
        node
            .GetTree()
            .CreateTimer((float) duration.TotalSeconds, pauseModeProcess)
            .OnTimeout()
            .Take(1)
            .ToTask();

    public static Task WaitForSeconds(this Node node, double seconds, bool pauseModeProcess = false) =>
        node.WaitFor(TimeSpan.FromSeconds(seconds), pauseModeProcess);

        #endregion
  }
}