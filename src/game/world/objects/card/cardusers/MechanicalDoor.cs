using Godot;

namespace Parry2.game.world.objects.card.cardusers
{
  [Tool]
  public class MechanicalDoor : Node2D, ICardUser
  {
    public CardColor Color
    {
      set => GetNode<CardIndicator>(nameof(CardIndicator)).Color = value;
    }

    public void OnCardCollected()
    {
#if DEBUG
      if (Engine.EditorHint) return;
#endif
      GetNode<AnimationPlayer>(nameof(AnimationPlayer))
          .Play("open");
    }
  }
}