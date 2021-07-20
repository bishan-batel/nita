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
        /// <param name="node">Godot object that the action is linked too</param>
        /// <param name="action">Action to be called once time is over</param>
        /// <param name="delay">Delay for action in seconds</param>
        public static void Dispatch(this Node node, Action action, float delay)
        {
            node.AddChild(new TimeoutTimer(action, delay));
        }

        class TimeoutTimer : Node
        {
            readonly Action _action;
            readonly float _delay;
            Timer _timer;

            public TimeoutTimer() : this(null, 0)
            {
            }

            public TimeoutTimer(Action action, float delay = 0)
            {
                _action = action ?? (() => { });
                _delay = delay;
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