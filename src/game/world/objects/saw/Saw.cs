using Godot;

namespace Parry2.game.world.objects.saw
{
    [Tool]
    public class Saw : Node2D
    {
        [Export] public float SpeedSnap = 15f / 2f, Duration = .25f;

        [Export]
        public bool SpinInEditor
        {
            get => _editorSpin;
            set
            {
                _editorSpin = value;
                if (!value) return;
                _timer.Stop();
                _timer.Start(Duration);
            }
        }

        Timer _timer;
        bool _editorSpin;

        public override void _Ready()
        {
            _timer = GetNodeOrNull<Timer>("Timer");
            _timer?.QueueFree();

            _timer = new Timer();
            AddChild(_timer);

            _timer.Start(Duration);
            _timer.Connect("timeout", this, nameof(Timeout));
        }

        public void Timeout()
        {
#if DEBUG
            if (!SpinInEditor && Engine.EditorHint) return;
#endif
            if (_timer is null)
            {
                _timer = new Timer();
                AddChild(_timer);
            }

            _timer.Start(Duration);
            RotationDegrees = (RotationDegrees + SpeedSnap) % 360;
        }
    }
}