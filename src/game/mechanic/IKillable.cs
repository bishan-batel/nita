namespace Nita.game.mechanic
{
  public interface IKillable
  {
    public int Health { set; get; }
    public void Kill();
  }
}