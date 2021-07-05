namespace Parry2.game.mechanic
{
    public interface IKillable
    {
        public int Health { set; get; }

        public void Kill();
    }
}