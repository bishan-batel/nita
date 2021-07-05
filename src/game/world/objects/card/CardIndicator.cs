using Godot;

namespace Parry2.game.world.objects.card
{
    [Tool]
    public class CardIndicator : Sprite, ICardUser
    {
        public CardColor Color
        {
            set => Frame = (int) value;
        }

        public void OnCardCollected()
        {
        }
    }
}