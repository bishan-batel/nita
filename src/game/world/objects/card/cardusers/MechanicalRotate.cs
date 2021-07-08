using System;
using Godot;

namespace Parry2.game.world.objects.card.cardusers
{
    [Tool]
    public class MechanicalRotate : MechanicalDoor, ICardUser
    {
        [Export]
        public float NormalRot
        {
            set
            {
                _normalRot = value;
                if (!_triggered)
                    RotationDegrees = value;
            }
            get => _normalRot;
        }

        [Export] public float TransformRot, Speed = .5f, DegreeSnap = 22.5f;

        float _normalRot, _rot;
        bool _triggered;

        public override void _Ready() =>
            _rot = NormalRot = NormalRot;

        public override void _Process(float delta)
        {
            float target = _triggered ? TransformRot : NormalRot;

            int dir = Mathf.Sign(target - _rot);
            _rot += dir * Speed;
            if (Mathf.Sign(target - _rot) != dir)
                _rot = target;

            RotationDegrees = Mathf.Stepify(_rot, DegreeSnap);

            // Keeps indicator in place
            GetNode<CardIndicator>(nameof(CardIndicator))
                .GlobalRotation = 0;
        }

        public new void OnCardCollected() => _triggered = true;
    }
}