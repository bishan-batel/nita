using Godot;

namespace Nita.game.world.objects.card.cardusers
{
  [Tool]
  public class MechanicalRotate : MechanicalDoor, ICardUser
  {
    float _normalRot, _rot;
    bool _triggered;

    [Export] public float TransformRot, Speed = .5f, DegreeSnap = 22.5f;

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

    public new void OnCardCollected()
    {
      _triggered = true;
    }

    public override void _Ready()
    {
      _rot = NormalRot = NormalRot;
    }

    public override void _Process(float delta)
    {
      float target = _triggered ? TransformRot : NormalRot;

      int dir = Mathf.Sign(target - _rot);
      _rot += dir * Speed * (delta * 100);
      if (Mathf.Sign(target - _rot) != dir)
        _rot = target;

      RotationDegrees = Mathf.Stepify(_rot, DegreeSnap);

      // Keeps indicator in place
      GetNode<CardIndicator>(nameof(CardIndicator))
          .GlobalRotation = 0;
    }
  }
}