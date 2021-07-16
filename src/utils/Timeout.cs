using System;
using System.Threading.Tasks;
using Godot;

namespace Parry2.utils
{
    public static class Timeout
    {
        /// <summary>
        /// Calls action after set time using godot's timer node 
        /// </summary>
        /// <param name="action">Action to be called once time is over</param>
        /// <param name="delay">Delay for action in seconds</param>
        public static void Dispatch(Action action, float delay) => Global
            .Singleton
            .AddChild(new TimeoutTimer(action, delay));

        class TimeoutTimer : Node
        {
            readonly Action _action;
            readonly float _delay;
            Timer _timer;

            public TimeoutTimer(Action action = null, float delay = 0)
            {
                _action = action ?? (() => { });
                _delay = delay;
            }


            public override void _Ready()
            {
                AddChild(_timer = new Timer {OneShot = true});
                _timer.Start(_delay);
                _timer.Connect("timeout", this, nameof(Invoke));
            }

            public void Invoke()
            {
                _action.Invoke();
                _timer.QueueFree();
                QueueFree();
            }
        }
    }
}