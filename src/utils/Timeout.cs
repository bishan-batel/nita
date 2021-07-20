using System;
using Godot;
using GodotRx;
using Object = Godot.Object;

namespace Parry2.utils
{
    public static class Timeout
    {
        /// <summary>
        ///     Calls action after set time using godot's timer node
        /// </summary>
        /// <param name="disposeWith">Godot object that the action is linked too</param>
        /// <param name="action">Action to be called once time is over</param>
        /// <param name="delay">Delay for action in seconds</param>
        // [Obsolete("Use GodotRX WaitFor instead", true)]
        public static void Dispatch(this Node disposeWith, Action action, float delay)
        {
            disposeWith.AddChild(new TimeoutTimer(disposeWith, action, delay));
        }

        // [Obsolete("Use GodotRX WaitFor instead", true)]
        class TimeoutTimer : Node
        {
            readonly Action _action;
            readonly float _delay;
            Object _disposeWith;
            Timer _timer;

            public TimeoutTimer() : this(null, null, 0)
            {
            }

            public TimeoutTimer(Object disposeWith, Action action, float delay = 0)
            {
                _action = action ?? (() => { });
                _delay = delay;
                _disposeWith = disposeWith;
            }

            public override void _Ready()
            {
                AddChild(_timer = new Timer {OneShot = true});
                _timer.Start(_delay);

                _timer
                    .OnTimeout()
                    .Subscribe(_ =>
                    {
                        _action.Invoke();
                        _timer.QueueFree();
                        QueueFree();
                    }).DisposeWith(this);
            }
        }
    }
}