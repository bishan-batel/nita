using System;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Nita.utils;

namespace Nita.game.detail.fireflies
{
  public class Firefly : Sprite
  {
    [Node("RayCast2D")] readonly RayCast2D _ray = null;
    [Node("VisibilityNotifier2D")] public readonly VisibilityNotifier2D Visibility = null;
    public FireflyGroup Source { set; get; }
    Vector2 _velocity;


    public override void _Ready()
    {
      this.Wire();

      // Apply offset
      var offset = new Vector2
      (
        (float) GD.RandRange(-10f, 10f),
        (float) GD.RandRange(-10f, 10f)
      );
      GlobalPosition += offset;

      GD.Randomize();
      GlobalRotation = (float) GD.RandRange(-Mathf.Pi, Mathf.Pi);
    }

    public void Update(float delta)
    {
      if (Source is null) return;

      GlobalPosition += _velocity * delta;
      GlobalRotation = _velocity.Angle();
    }
  }
}