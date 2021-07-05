using System;
using System.Runtime.Serialization;
using Godot;
using Parry2.game.mechanic.hittable;
using Parry2.managers.save;

namespace Parry2.game.world.objects.shroomvine_wheel
{
    public class ShroomvineWheel : Node2D
    {
        float _angVel;
        [Export(PropertyHint.Range, "0,1")] public float AngleFriction = 0.8f;
        [Export] public float HitImpulse = 100f, SpeedSnap;
        [Export] public bool StartWithRandomAngle = true;

        float _angle;

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

        public void _on_HittableArea_OnHit(HitInformation hitInfo) =>
            _angVel -= HitImpulse;
    }
}