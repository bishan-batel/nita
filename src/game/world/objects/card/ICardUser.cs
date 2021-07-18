namespace Parry2.game.world.objects.card
{
    public interface ICardUser
    {
        public CardColor Color { set; }
        void OnCardCollected();
    }
}