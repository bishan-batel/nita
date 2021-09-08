using System;
using System.Reactive.Linq;
using GDMechanic.Wiring;
using GDMechanic.Wiring.Attributes;
using Godot;
using Nita.addons.godotrx;
using Nita.game.world.actors.player;

namespace Nita.game.world.objects.spring
{
  public class Spring : Area2D
  {
    [Export] public float Impulse;
    [Node("AnimationPlayer")] readonly AnimationPlayer _player = null;

    public override void _Ready()
    {
      this.Wire();
      this
          .OnBodyEntered()
          .Cast<PlayerShroom>()
          .Subscribe(ApplyImpulse)
          .DisposeWith(this);
    }

    void ApplyImpulse(PlayerShroom body)
    {
      if (_player.IsPlaying()) return;

      Vector2 impulse = GetImpulse() * Impulse;
      impulse.y -= body.Velocity.y;

      body.ApplyForce(impulse);
      body.IsJumpHold = true;
      _player.Play("activate");
    }

    public Vector2 GetImpulse()
    {
      var heading = new Vector2(Mathf.Cos(GlobalRotation), Mathf.Sin(GlobalRotation));
      return heading.Rotated(Mathf.Pi / 2);
    }
  }
}