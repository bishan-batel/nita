using System.Linq;
using System.Reactive.Linq;
using Godot;
using Nita.addons.godotrx;

namespace Nita.game.detail.fireflies
{
  public class FireflyGroup : Node2D
  {
    public static readonly PackedScene FireflyScene = GD.Load<PackedScene>("res://src/game/detail/fireflies/Firefly.tscn");
    [Export] public bool SimulateOffScreen { set; get; }
    [Export] public float Speed, TurnAverageSpeed = .1f;

    public Vector2 AveragePosition { private set; get; }

    public override void _Ready()
    {
      for (var i = 0; i < 20; i++)
      {
        var firefly = FireflyScene.Instance<Firefly>();
        firefly.Source = this;

        AddChild(firefly);
      }

      AveragePosition = GlobalPosition;
    }

    public override void _PhysicsProcess(float delta)
    {
      Vector2 average = GlobalPosition;
      var count = 1;

      GetChildren()
          .Cast<Firefly>()
          .Where(firefly => SimulateOffScreen || firefly.Visibility.IsOnScreen())
          .ToList()
          .ForEach(firefly =>
          {
            firefly.Update(delta);
            average += firefly.GlobalPosition;
            count++;
          });
      average /= count;
      AveragePosition = average;
    }
  }
}