using Godot;

namespace Parry2.game.mechanic.interactable
{
  public interface IInteractable
  {
    public void OnInteract(InteractionInfo info);
  }

  public class InteractionInfo : Resource
  {
    public readonly Node User;
    
    public InteractionInfo(Node user)
    {
      User = user;
    }
  }
}