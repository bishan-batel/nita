using Godot;
using Nita.game.mechanic.hittable;

namespace Nita.game.world.objects.shroomvine_wheel
{
  public class ShroomvineWheel : Node2D
  {
	float _angle;
	float _angVel;
	[Export(PropertyHint.Range, "0,1")] public float AngleFriction = 0.8f;
	[Export] public float HitImpulse = 100f, SpeedSnap;
	[Export] public bool StartWithRandomAngle = true;

	public override void _Ready()
	{
	  if (!StartWithRandomAngle) return;
	  var rng = new RandomNumberGenerator();
	  rng.Randomize();
	  _angle = rng.RandfRange(0.0f, 360f);

	  // Snaps angle to multiple of SpeedSnap
	  _angle = Mathf.Round(_angle / SpeedSnap) * SpeedSnap;
	}

	public override void _Process(float delta)
	{
	  _angVel *= AngleFriction;
	  _angle += _angVel;

	  RotationDegrees = Mathf.Round(_angle / SpeedSnap) * SpeedSnap;
	}

	public void _on_HittableArea_OnHit(HitInformation hitInfo)
	{
	  _angVel -= HitImpulse;
	}
  }
}
